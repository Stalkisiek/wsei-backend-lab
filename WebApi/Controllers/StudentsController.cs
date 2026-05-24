using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CoreApp.Services;
using CoreApp.Dto;
using CoreApp.Models;

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

    [HttpPost("{id:guid}/grades")]
    public async Task<IActionResult> AddGrade(Guid id, [FromBody] CoreApp.Dto.GradeDto dto)
    {
        var existing = await _service.GetStudentById(id);
        if (existing == null) return NotFound();
        try
        {
            var created = await _service.AddGrade(id, dto);
            var result = new CoreApp.Dto.GradeDto
            {
                Id = created.Id,
                CourseId = created.Course?.Id ?? Guid.Empty,
                GradeValue = created.GradeValue.Value(),
                GradeType = created.GradeType,
                LecturerId = created.Lecturer?.Id,
                AcademicYearId = created.AcademicYear?.Id,
                Date = created.Date
            };
            return CreatedAtAction(nameof(GetGrades), new { studentId = id }, result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
            catch (CoreApp.Exceptions.LecturerNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }

    [HttpGet("{studentId:guid}/grades")]
    public async Task<IActionResult> GetGrades(Guid studentId)
    {
        var student = await _service.GetById(studentId);
        if (student == null) return NotFound();
        var grades = await _service.GetGradesAsync(studentId);
        return Ok(grades);
    }

    [HttpPut("{studentId:guid}/grades/{gradeId:guid}")]
    public async Task<IActionResult> UpdateGrade(Guid studentId, Guid gradeId, [FromBody] CoreApp.Dto.GradeUpdateDto dto)
    {
        var student = await _service.GetById(studentId);
        if (student == null) return NotFound();
        try
        {
            var updated = await _service.UpdateGrade(studentId, gradeId, dto);
            var result = new CoreApp.Dto.GradeDto
            {
                  Id = updated.Id,
                CourseId = updated.Course?.Id ?? Guid.Empty,
                GradeValue = updated.GradeValue.Value(),
                GradeType = updated.GradeType,
                LecturerId = updated.Lecturer?.Id,
                AcademicYearId = updated.AcademicYear?.Id,
                Date = updated.Date
            };
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }
}

