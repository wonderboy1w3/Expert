using Expert.Web.DTOs;
using Expert.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Expert.Web.Controllers;

public class GradesController : Controller
{
    private readonly IGradeService _gradeService;
    private readonly IUserService _userService;

    public GradesController(IGradeService gradeService, IUserService userService)
    {
        _gradeService = gradeService;
        _userService = userService;
    }

    public async Task<IActionResult> Create(GradeCreationDto dto)
    {
        var result = await _gradeService.CreateAsync(dto);
        var user = await _userService.GetAsync(dto.SetterId);
        return RedirectToAction("Index", "Users", user);
    }

    [HttpGet("index/{userId:long}")]
    public async Task<IActionResult> Index(long userId)
    {
        return View(await _gradeService.GetAllAsync(userId));
    }
}