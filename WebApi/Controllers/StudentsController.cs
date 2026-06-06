using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreApp.Authorization;
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
    [Authorize(Roles = "Administrator,Lecturer,DeanOffice")]
    public async Task<IActionResult> GetAllStudents(int page = 1, int size = 10)
    {
        var result = await _service.FindAllStudentsPagedAsync(page, size);
        return Ok(result);
    }

    [HttpGet("id/{id:guid}")]
    public async Task<IActionResult> GetStudentById(Guid id)
    {
        var dto = await _service.GetStudentByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("student-id/{studentId}")]
    public async Task<IActionResult> GetStudentByStudentId(string studentId)
    {
        var entityId = await _service.ResolveStudentEntityIdByStudentIdAsync(studentId);
        if (!entityId.HasValue) return NotFound();

        var dto = await _service.GetStudentByIdAsync(entityId.Value);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = nameof(AppPolicies.Administrator))]
    public async Task<IActionResult> Create([FromBody] StudentCreateDto dto)
    {
        try
        {
            var entity = await _service.AddStudent(dto);
            var result = await _service.GetStudentByIdAsync(entity.Id);
            return CreatedAtAction(nameof(GetStudentById), new { id = entity.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPut("id/{id:guid}")]
    [Authorize(Policy = nameof(AppPolicies.Administrator))]
    public async Task<IActionResult> UpdateStudentById(Guid id, [FromBody] StudentUpdateDto dto)
    {
        // sprawdź czy student istnieje
        var existing = await _service.GetStudentByIdAsync(id);
        if (existing == null) return NotFound();

        // wykonaj aktualizację
        var updated = await _service.UpdateStudentAsync(id, dto);

        // przygotuj odpowiedź jako StudentSummaryDto
        var summary = new StudentSummaryDto
        {
            Id = updated.Id,
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

    [HttpPut("student-id/{studentId}")]
    [Authorize(Policy = nameof(AppPolicies.Administrator))]
    public async Task<IActionResult> UpdateStudentByStudentId(string studentId, [FromBody] StudentUpdateDto dto)
    {
        var entityId = await _service.ResolveStudentEntityIdByStudentIdAsync(studentId);
        if (!entityId.HasValue) return NotFound();

        return await UpdateStudentById(entityId.Value, dto);
    }

    [HttpPost("id/{id:guid}/grades")]
    [Authorize(Policy = nameof(AppPolicies.Administrator))]
    public async Task<IActionResult> AddGradeById(Guid id, [FromBody] GradeDto dto)
    {
        var existing = await _service.GetStudentById(id);
        if (existing == null) return NotFound();
        try
        {
            var created = await _service.AddGrade(id, dto);
            var result = new GradeDto
            {
                Id = created.Id,
                CourseId = created.Course.Id,
                GradeValue = created.GradeValue.Value(),
                GradeType = created.GradeType,
                LecturerId = created.Lecturer?.Id,
                AcademicYearId = created.AcademicYear?.Id,
                Date = created.Date
            };
            return CreatedAtAction(nameof(GetGradesById), new { id }, result);
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

    [HttpPost("student-id/{studentId}/grades")]
    [Authorize(Policy = nameof(AppPolicies.Administrator))]
    public async Task<IActionResult> AddGradeByStudentId(string studentId, [FromBody] GradeDto dto)
    {
        var entityId = await _service.ResolveStudentEntityIdByStudentIdAsync(studentId);
        if (!entityId.HasValue) return NotFound();

        return await AddGradeById(entityId.Value, dto);
    }

    [HttpGet("id/{id:guid}/grades")]
    public async Task<IActionResult> GetGradesById(Guid id)
    {
        var student = await _service.GetById(id);
        if (student == null) return NotFound();
        var grades = await _service.GetGradesAsync(id);
        return Ok(grades);
    }

    [HttpGet("student-id/{studentId}/grades")]
    public async Task<IActionResult> GetGradesByStudentId(string studentId)
    {
        var entityId = await _service.ResolveStudentEntityIdByStudentIdAsync(studentId);
        if (!entityId.HasValue) return NotFound();

        return await GetGradesById(entityId.Value);
    }

    [HttpPut("id/{id:guid}/grades/{gradeId:guid}")]
    [Authorize(Policy = nameof(AppPolicies.Administrator))]
    public async Task<IActionResult> UpdateGradeById(Guid id, Guid gradeId, [FromBody] GradeUpdateDto dto)
    {
        var student = await _service.GetById(id);
        if (student == null) return NotFound();
        try
        {
            var updated = await _service.UpdateGrade(id, gradeId, dto);
            var result = new GradeDto
            {
                  Id = updated.Id,
                CourseId = updated.Course.Id,
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

    [HttpPut("student-id/{studentId}/grades/{gradeId:guid}")]
    [Authorize(Policy = nameof(AppPolicies.Administrator))]
    public async Task<IActionResult> UpdateGradeByStudentId(string studentId, Guid gradeId, [FromBody] GradeUpdateDto dto)
    {
        var entityId = await _service.ResolveStudentEntityIdByStudentIdAsync(studentId);
        if (!entityId.HasValue) return NotFound();

        return await UpdateGradeById(entityId.Value, gradeId, dto);
    }
}

