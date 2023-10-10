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
using Expert.Web.Models;

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
    
    public async Task<IActionResult> Index(UserResultDto dto = null)
    {
        var users = await _userService.GetAllAsync();
        var grades = await _gradeService.GetAllAsync();
        return View(new UserViewModel() { User = dto, Users = users, Grades = grades});
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
            ModelState.AddModelError("", "����� ��� ������ ������������ ������� �������");
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
        return RedirectToAction("index", user);
    }
}