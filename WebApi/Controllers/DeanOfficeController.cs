using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreApp.Authorization;
using CoreApp.Services;
using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Repositories;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/dean-office")]
[Authorize(Policy = nameof(AppPolicies.DeanOffice))]
public class DeanOfficeController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILecturerService _lecturerService;
    private readonly ILecturerRepository _lecturerRepository;
    private readonly IDegreeProgramService _degreeProgramService;
    private readonly ICourseManagementService _courseManagementService;

    public DeanOfficeController(
        IStudentService studentService,
        ILecturerService lecturerService,
        ILecturerRepository lecturerRepository,
        IDegreeProgramService degreeProgramService,
        ICourseManagementService courseManagementService)
    {
        _studentService = studentService;
        _lecturerService = lecturerService;
        _lecturerRepository = lecturerRepository;
        _degreeProgramService = degreeProgramService;
        _courseManagementService = courseManagementService;
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetAllStudents(int page = 1, int size = 50)
    {
        var result = await _studentService.FindAllStudentsPagedAsync(page, size);
        return Ok(result);
    }

    [HttpPost("students")]
    public async Task<IActionResult> RegisterStudent([FromBody] StudentCreateDto dto)
    {
        try
        {
            var entity = await _studentService.AddStudent(dto);
            var result = await _studentService.GetStudentByIdAsync(entity.Id);
            return CreatedAtAction(nameof(GetStudentDetails), new { id = entity.Id }, result);
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

    [HttpPut("students/{id:guid}")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] StudentUpdateDto dto)
    {
        try
        {
            var existing = await _studentService.GetStudentByIdAsync(id);
            if (existing == null) return NotFound(new { error = "Student not found" });

            var updated = await _studentService.UpdateStudentAsync(id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }

    [HttpPut("students/{id:guid}/status")]
    public async Task<IActionResult> UpdateStudentStatus(Guid id, [FromBody] UpdateStatusDto dto)
    {
        try
        {
            var existing = await _studentService.GetStudentByIdAsync(id);
            if (existing == null) return NotFound(new { error = "Student not found" });

            var updated = await _studentService.UpdateStudentStatusAsync(id, dto.Status);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }

    [HttpGet("students/{id:guid}")]
    public async Task<IActionResult> GetStudentDetails(Guid id)
    {
        var dto = await _studentService.GetStudentByIdAsync(id);
        if (dto == null) return NotFound(new { error = "Student not found" });
        return Ok(dto);
    }

    [HttpPost("lecturers")]
    public async Task<IActionResult> RegisterLecturer([FromBody] LecturerCreateDto dto)
    {
        try
        {
            var result = await _lecturerService.CreateLecturerAsync(dto);
            return CreatedAtAction(nameof(GetLecturerDetails), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }

    [HttpPut("lecturers/{id:guid}")]
    public async Task<IActionResult> UpdateLecturer(Guid id, [FromBody] LecturerUpdateDto dto)
    {
        try
        {
            var existing = await _lecturerService.GetLecturerByIdAsync(id);
            if (existing == null) return NotFound(new { error = "Lecturer not found" });

            var updated = await _lecturerService.UpdateLecturerAsync(id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }

    [HttpGet("lecturers/{id:guid}")]
    public async Task<IActionResult> GetLecturerDetails(Guid id)
    {
        var dto = await _lecturerService.GetLecturerByIdAsync(id);
        if (dto == null) return NotFound(new { error = "Lecturer not found" });
        return Ok(dto);
    }

    [HttpGet("lecturers/{id:guid}/courses")]
    public async Task<IActionResult> GetLecturerCourses(Guid id)
    {
        try
        {
            var existing = await _lecturerService.GetLecturerByIdAsync(id);
            if (existing == null) return NotFound(new { error = "Lecturer not found" });

            var courses = await _lecturerService.GetCoursesByLecturerAsync(id);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }

    [HttpGet("lecturers")]
    public async Task<IActionResult> GetAllLecturers()
    {
        var lecturers = await _lecturerRepository.FindAllAsync();
        var result = lecturers.Select(l => new
        {
            l.Id,
            l.FirstName,
            l.LastName,
            Email = l.Email.ToString(),
            l.Title,
            l.Faculty,
            Pesel = l.Pesel != null ? l.Pesel.ToString() : null
        });

        return Ok(result);
    }

    [HttpPost("degree-programs")]
    public async Task<IActionResult> CreateDegreeProgram([FromBody] DegreeProgramCreateDto dto)
    {
        try
        {
            var created = await _degreeProgramService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDegreeProgramReport), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("degree-programs")]
    public async Task<IActionResult> GetAllDegreePrograms()
    {
        var programs = await _degreeProgramService.GetAllAsync();
        return Ok(programs);
    }

    [HttpGet("degree-programs/{id:guid}/report")]
    public async Task<IActionResult> GetDegreeProgramReport(Guid id)
    {
        var report = await _degreeProgramService.GetReportAsync(id);
        if (report == null) return NotFound(new { error = "Degree program not found" });
        return Ok(report);
    }

    [HttpPost("courses")]
    public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto dto)
    {
        try
        {
            var created = await _courseManagementService.CreateCourseAsync(dto);
            return CreatedAtAction(nameof(GetCourseReport), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("courses/{id:guid}/lecturer/{lecturerId:guid}")]
    public async Task<IActionResult> AssignLecturer(Guid id, Guid lecturerId)
    {
        try
        {
            var updated = await _courseManagementService.AssignLecturerAsync(id, lecturerId);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("courses/{id:guid}/students/{studentId:guid}")]
    public async Task<IActionResult> EnrollStudent(Guid id, Guid studentId)
    {
        try
        {
            var updated = await _courseManagementService.EnrollStudentAsync(id, studentId);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpDelete("courses/{id:guid}/students/{studentId:guid}")]
    public async Task<IActionResult> UnenrollStudent(Guid id, Guid studentId)
    {
        try
        {
            var updated = await _courseManagementService.UnenrollStudentAsync(id, studentId);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("courses/{id:guid}/report")]
    public async Task<IActionResult> GetCourseReport(Guid id)
    {
        var report = await _courseManagementService.GetCourseReportAsync(id);
        if (report == null) return NotFound(new { error = "Course not found" });
        return Ok(report);
    }
}


