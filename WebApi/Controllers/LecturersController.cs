using Microsoft.AspNetCore.Mvc;
using CoreApp.Repositories;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/lecturers")]
public class LecturersController : ControllerBase
{
    private readonly ILecturerRepository _repo;

    public LecturersController(ILecturerRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _repo.FindAllAsync();
        var result = items.Select(l => new {
            id = l.Id,
            firstName = l.FirstName,
            lastName = l.LastName,
            email = l.Email,
            title = l.Title,
            faculty = l.Faculty
        }).ToList();
        return Ok(result);
    }
}

