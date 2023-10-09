using Expert.Web.DTOs;
using Expert.Web.Interfaces;
using Expert.Web.Services;
using Microsoft.AspNetCore.Mvc;

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
    
    public async Task<IActionResult> Index()
    {
        return View(await _userService.GetAllAsync(UserType.Getter));
    }

    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(string login, string password)
    {
        var user = await _userService.CheckAsync(login, password);
        if (user is not null)
        {
            return RedirectToAction("Index");
        }    
        return Redirect("/login");
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
            return Redirect("/register");
        return Redirect("/index");
    }
    
    public async Task<IActionResult> User(long id)
    {
        return View((await _userService.GetAsync(id), await _gradeService.GetAsync(id)));
    }
}