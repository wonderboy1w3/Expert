using Expert.Web.DTOs;
using Expert.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Expert.Web.Controllers;

public class GradesController : Controller
{
    private readonly IGradeService _gradeService;

    public GradesController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    public async Task<IActionResult> Create(GradeCreationDto dto)
    {
        var result = await _gradeService.CreateAsync(dto);
        return Redirect($"index/{dto.GetterId}");
    }

    [HttpGet("index/{userId:long}")]
    public async Task<IActionResult> Index(long userId)
    {
        return View(await _gradeService.GetAllAsync(userId));
    }
}