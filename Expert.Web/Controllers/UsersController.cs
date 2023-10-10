using Expert.Web.DTOs;
using Expert.Web.Interfaces;
using Expert.Web.Models.Account;
using Expert.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Expert.Web.Controllers;

public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IGradeService _gradeService;
    
    public UsersController(IUserService userService, IGradeService gradeService)
    {
        _userService = userService;
        _gradeService = gradeService;
    }
    
    public async Task<IActionResult> Index(UserResultDto dto = null, long? userId = null, int? score = null)
    {

        var users = await _userService.GetAllAsync();
        if(userId is not null)
        {
            var user = users.FirstOrDefault(t => t.Id.Equals(userId));

        }
        return View((dto ,users));
    }

    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost()]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var returnUrl = TempData["ReturnUrl"] as string;

        var user = await _userService.CheckAsync(model.UserName, model.Password);
        if (user is null)
        {
            ModelState.AddModelError("", "Ћогин или пароль пользовател€ указаны неверно");
            return View(model);
        }

        return RedirectToAction("index", user);
    }


    public IActionResult Register()
    {
        UserCreationDto dto = new();
        return View(dto);
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(UserCreationDto dto)
    {
        var user = await _userService.CreateAsync(dto);
        if(user is null)    
            return RedirectToAction("register");
        return RedirectToAction("index");
    }
    
    public async Task<IActionResult> User(long id)
    {
        return View((await _userService.GetAsync(id), await _gradeService.GetAsync(id)));
    }
}