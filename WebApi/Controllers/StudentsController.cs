using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CoreApp.Services;
using CoreApp.Dto;

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var dto = await _service.GetStudentByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentCreateDto dto)
    {
        var entity = await _service.AddStudent(dto);
        var result = await _service.GetStudentByIdAsync(entity.Id);
        return CreatedAtAction(nameof(GetStudent), new { id = entity.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] StudentUpdateDto dto)
    {
        // sprawdź czy student istnieje
        var existing = await _service.GetStudentByIdAsync(id);
        if (existing == null) return NotFound();

        // wykonaj aktualizację
        var updated = await _service.UpdateStudentAsync(id, dto);

        // przygotuj odpowiedź jako StudentSummaryDto
        var summary = new StudentSummaryDto
        {
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            Email = updated.Email,
            StudentId = updated.StudentId,
            ProgramName = updated.ProgramName,
            YearOfStudy = updated.YearOfStudy,
            Status = updated.Status
        };

        return Ok(summary);
    }
}

