using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CoreApp.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents(int page = 1, int size = 10)
    {
        var result = await _service.FindAllStudentsPagedAsync(page, size);
        return Ok(result);
    }
}

