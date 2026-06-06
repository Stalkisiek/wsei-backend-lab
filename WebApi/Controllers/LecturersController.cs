using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CoreApp.Repositories;
using CoreApp.Authorization;
using CoreApp.Services;
using CoreApp.Dto;
using CoreApp.Models;
using FluentValidation;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/lecturers")]
public class LecturersController : ControllerBase
{
    private readonly ILecturerRepository _lecturerRepo;
    private readonly ILecturerService _lecturerService;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IValidator<LecturerGradeUpdateDto> _gradeValidator;

    public LecturersController(
        ILecturerRepository repo,
        ILecturerService lecturerService,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        IGradeRepository gradeRepository,
        IValidator<LecturerGradeUpdateDto> gradeValidator)
    {
        _lecturerRepo = repo;
        _lecturerService = lecturerService;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _gradeRepository = gradeRepository;
        _gradeValidator = gradeValidator;
    }

    [HttpGet("all/students")]
    [Authorize(Roles = "Administrator,Lecturer,DeanOffice")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStudents(int page = 1, int size = 50)
    {
        var result = await _studentRepository.FindPagedAsync(page, size);
        var items = result.Items.Select(s => new LecturerStudentDto
        {
            Id = s.Id,
            StudentId = s.StudentId,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email.ToString(),
            Pesel = s.Pesel?.ToString() ?? "N/A",
            YearOfStudy = s.YearOfStudy,
            ProgramName = s.ProgramName
        }).ToList();

        return Ok(new CoreApp.Repositories.PagedResult<LecturerStudentDto>(items, result.TotalCount, result.Page, result.PageSize));
    }

    [HttpGet("all/courses")]
    [Authorize(Roles = "Administrator,Lecturer,DeanOffice")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCourses(int page = 1, int size = 50)
    {
        var result = await _courseRepository.FindPagedAsync(page, size);
        var items = result.Items.Select(c => new LecturerCourseDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            EctsCredits = c.EctsCredits,
            CompletionType = c.CompletionType.ToString(),
            EnrolledStudentsCount = c.Enrollments?.Count ?? 0
        }).ToList();

        return Ok(new CoreApp.Repositories.PagedResult<LecturerCourseDto>(items, result.TotalCount, result.Page, result.PageSize));
    }

    [HttpGet("all/grades")]
    [Authorize(Roles = "Administrator,Lecturer,DeanOffice")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllGrades(int page = 1, int size = 50)
    {
        var result = await _gradeRepository.FindPagedAsync(page, size);
        var items = result.Items.Select(g => new GradeWithHistoryDto
        {
            Id = g.Id,
            Value = (double)g.GradeValue,
            Type = g.GradeType.ToString(),
            Date = g.Date,
            LecturerName = g.Lecturer != null ? $"{g.Lecturer.FirstName} {g.Lecturer.LastName}" : null,
            CreatedBy = g.CreatedBy,
            CreatedAt = g.CreatedAt,
            ModifiedBy = g.ModifiedBy,
            ModifiedAt = g.ModifiedAt,
            ChangeHistory = (g.ChangeHistory ?? new List<GradeChangeHistory>()).Select(h => new GradeChangeHistoryDto
            {
                Id = h.Id,
                PreviousValue = h.PreviousValue != null ? (double)h.PreviousValue : null,
                NewValue = (double)h.NewValue,
                ChangedBy = h.ChangedBy,
                ChangedAt = h.ChangedAt
            }).ToList()
        }).ToList();

        return Ok(new CoreApp.Repositories.PagedResult<GradeWithHistoryDto>(items, result.TotalCount, result.Page, result.PageSize));
    }
    
    [HttpGet]
    [Authorize(Roles = "Administrator,Lecturer,DeanOffice")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var items = await _lecturerRepo.FindAllAsync();
        var result = items.Select(l => new {
            id = l.Id,
            firstName = l.FirstName,
            lastName = l.LastName,
            email = l.Email.ToString(),
            title = l.Title,
            faculty = l.Faculty,
            pesel = l.Pesel != null ? l.Pesel.ToString() : "N/A"
        }).ToList();
        return Ok(result);
    }
    
    [HttpGet("{lecturerId}")]
    [Authorize(Policy = nameof(AppPolicies.Lecturer))]
    [ProducesResponseType(typeof(LecturerDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(Guid lecturerId)
    {
        var accessResult = await ValidateLecturerAccessAsync(lecturerId);
        if (accessResult != null) return accessResult;

        var profile = await _lecturerService.GetLecturerProfileAsync(lecturerId);
        if (profile == null)
            return NotFound(new { error = "Lecturer not found" });
        
        return Ok(profile);
    }
    
    [HttpGet("{lecturerId}/courses")]
    [Authorize(Policy = nameof(AppPolicies.LecturerOrDeanOffice))]
    [ProducesResponseType(typeof(IEnumerable<LecturerCourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourses(Guid lecturerId)
    {
        var accessResult = await ValidateLecturerAccessAsync(lecturerId);
        if (accessResult != null) return accessResult;

        var courses = await _lecturerService.GetCoursesByLecturerAsync(lecturerId);
        return Ok(courses);
    }
    
    [HttpGet("{lecturerId}/courses/{courseId}/students")]
    [Authorize(Policy = nameof(AppPolicies.LecturerOrDeanOffice))]
    [ProducesResponseType(typeof(IEnumerable<LecturerStudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentsInCourse(Guid lecturerId, Guid courseId)
    {
        var accessResult = await ValidateLecturerAccessAsync(lecturerId);
        if (accessResult != null) return accessResult;

        try
        {
            var students = await _lecturerService.GetStudentsByCourseAsync(lecturerId, courseId);
            return Ok(students);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
    
    [HttpGet("{lecturerId}/students/{studentId}/courses/{courseId}/grades")]
    [Authorize(Policy = nameof(AppPolicies.LecturerOrDeanOffice))]
    [ProducesResponseType(typeof(IEnumerable<GradeWithHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentGrades(Guid lecturerId, Guid studentId, Guid courseId)
    {
        var accessResult = await ValidateLecturerAccessAsync(lecturerId);
        if (accessResult != null) return accessResult;

        try
        {
            var grades = await _lecturerService.GetStudentGradesAsync(lecturerId, studentId, courseId);
            return Ok(grades);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
    
    [HttpPost("{lecturerId}/students/{studentId}/courses/{courseId}/grades")]
    [Authorize(Policy = nameof(AppPolicies.LecturerOrDeanOffice))]
    [ProducesResponseType(typeof(GradeWithHistoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddGrade(Guid lecturerId, Guid studentId, Guid courseId, [FromBody] LecturerGradeUpdateDto dto)
    {
        var accessResult = await ValidateLecturerAccessAsync(lecturerId);
        if (accessResult != null) return accessResult;

        var validationResult = await _gradeValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        try
        {
            var changedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var grade = await _lecturerService.AddGradeAsync(lecturerId, studentId, courseId, dto, changedBy);
            return CreatedAtAction(nameof(GetStudentGrades), new { lecturerId, studentId, courseId }, grade);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
    
    [HttpPut("{lecturerId}/grades/{gradeId}")]
    [Authorize(Policy = nameof(AppPolicies.LecturerOrDeanOffice))]
    [ProducesResponseType(typeof(GradeWithHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGrade(Guid lecturerId, Guid gradeId, [FromBody] LecturerGradeUpdateDto dto)
    {
        var accessResult = await ValidateLecturerAccessAsync(lecturerId);
        if (accessResult != null) return accessResult;

        var validationResult = await _gradeValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        try
        {
            var changedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var grade = await _lecturerService.UpdateGradeAsync(lecturerId, gradeId, dto, changedBy);
            return Ok(grade);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    private async Task<IActionResult?> ValidateLecturerAccessAsync(Guid lecturerId)
    {
        var lecturer = await _lecturerRepo.FindByIdAsync(lecturerId);
        if (lecturer == null)
            return NotFound(new { error = "Lecturer not found" });

        if (User.IsInRole("DeanOffice") || User.IsInRole("Administrator"))
            return null;

        if (!User.IsInRole("Lecturer"))
            return Forbid();

        var email = User.FindFirstValue(ClaimTypes.Email);
        var firstName = User.FindFirstValue(ClaimTypes.GivenName);
        var lastName = User.FindFirstValue(ClaimTypes.Surname);

        var currentLecturer = (await _lecturerRepo.FindAllAsync())
            .FirstOrDefault(l =>
                (email != null && string.Equals(l.Email.ToString(), email, StringComparison.OrdinalIgnoreCase)) ||
                (firstName != null && lastName != null &&
                 string.Equals(l.FirstName, firstName, StringComparison.OrdinalIgnoreCase) &&
                 string.Equals(l.LastName, lastName, StringComparison.OrdinalIgnoreCase)));

        if (currentLecturer == null || currentLecturer.Id != lecturerId)
            return Forbid();

        return null;
    }
}

