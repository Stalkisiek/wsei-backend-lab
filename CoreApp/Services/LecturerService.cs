using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Repositories;
using AutoMapper;

namespace CoreApp.Services;

public class LecturerService : ILecturerService
{
    private readonly IUniversityUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LecturerService(IUniversityUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LecturerCourseDto>> GetCoursesByLecturerAsync(Guid lecturerId)
    {
        var courses = await _unitOfWork.Lecturers.GetCoursesByLecturerAsync(lecturerId);
        
        var result = new List<LecturerCourseDto>();
        foreach (var course in courses)
        {
            var enrolledCount = course.Enrollments?.Count ?? 0;
            result.Add(new LecturerCourseDto
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                EctsCredits = course.EctsCredits,
                CompletionType = course.CompletionType.ToString(),
                EnrolledStudentsCount = enrolledCount
            });
        }

        return result;
    }

    public async Task<IEnumerable<LecturerStudentDto>> GetStudentsByCourseAsync(Guid lecturerId, Guid courseId)
    {
        var students = await _unitOfWork.Lecturers.GetStudentsByCourseAsync(lecturerId, courseId);

        var result = new List<LecturerStudentDto>();
        foreach (var student in students)
        {
            result.Add(new LecturerStudentDto
            {
                Id = student.Id,
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email.ToString(),
                Pesel = student.Pesel?.ToString() ?? "N/A",
                YearOfStudy = student.YearOfStudy,
                ProgramName = student.ProgramName
            });
        }

        return result;
    }

    public async Task<IEnumerable<GradeWithHistoryDto>> GetStudentGradesAsync(Guid lecturerId, Guid studentId, Guid courseId)
    {

        var grades = await _unitOfWork.Grades.FindByStudentAndCourseAsync(studentId, courseId);
        
        var result = new List<GradeWithHistoryDto>();
        foreach (var grade in grades)
        {
            var history = await _unitOfWork.Grades.GetChangeHistoryAsync(grade.Id);
            
            result.Add(new GradeWithHistoryDto
            {
                Id = grade.Id,
                Value = (double)grade.GradeValue,
                Type = grade.GradeType.ToString(),
                Date = grade.Date,
                LecturerName = grade.Lecturer != null ? $"{grade.Lecturer.FirstName} {grade.Lecturer.LastName}" : null,
                CreatedBy = grade.CreatedBy,
                CreatedAt = grade.CreatedAt,
                ModifiedBy = grade.ModifiedBy,
                ModifiedAt = grade.ModifiedAt,
                ChangeHistory = history.Select(h => new GradeChangeHistoryDto
                {
                    Id = h.Id,
                    PreviousValue = h.PreviousValue != null ? (double)h.PreviousValue : null,
                    NewValue = (double)h.NewValue,
                    ChangedBy = h.ChangedBy,
                    ChangedAt = h.ChangedAt
                }).ToList()
            });
        }

        return result;
    }

    public async Task<GradeWithHistoryDto> AddGradeAsync(Guid lecturerId, Guid studentId, Guid courseId, LecturerGradeUpdateDto dto, string changedBy)
    {
        var teaches = await _unitOfWork.Lecturers.TeachesCourseAsync(lecturerId, courseId);
        if (!teaches)
            throw new UnauthorizedAccessException($"Lecturer {lecturerId} does not teach course {courseId}");

        var student = await _unitOfWork.Students.FindByIdAsync(studentId);
        if (student == null)
            throw new InvalidOperationException($"Student {studentId} not found");

        var course = await _unitOfWork.Courses.FindByIdAsync(courseId);
        if (course == null)
            throw new InvalidOperationException($"Course {courseId} not found");

        var gradeValue = GradeExtensions.From(dto.GradeValue);
        var gradeType = Enum.Parse<GradeType>(dto.GradeType);

        var grade = new Grade
        {
            Id = Guid.NewGuid(),
            Student = student,
            Course = course,
            Lecturer = await _unitOfWork.Lecturers.FindByIdAsync(lecturerId),
            GradeValue = gradeValue,
            GradeType = gradeType,
            Date = dto.Date ?? DateTime.UtcNow,
            CreatedBy = changedBy,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Grades.AddAsync(grade);
        await _unitOfWork.SaveChangesAsync();
        
        await _unitOfWork.Grades.AddGradeChangeAsync(grade.Id, null, gradeValue, changedBy);

        return await GetGradeWithHistoryAsync(grade.Id);
    }

    public async Task<GradeWithHistoryDto> UpdateGradeAsync(Guid lecturerId, Guid gradeId, LecturerGradeUpdateDto dto, string changedBy)
    {
        var grade = await _unitOfWork.Grades.FindByIdAsync(gradeId);
        if (grade == null)
            throw new InvalidOperationException($"Grade {gradeId} not found");

        var teaches = await _unitOfWork.Lecturers.TeachesCourseAsync(lecturerId, grade.Course.Id);
        if (!teaches)
            throw new UnauthorizedAccessException($"Lecturer {lecturerId} does not teach the course for this grade");

        var previousValue = grade.GradeValue;
        var newGradeValue = GradeExtensions.From(dto.GradeValue);
        var gradeType = Enum.Parse<GradeType>(dto.GradeType);

        grade.GradeValue = newGradeValue;
        grade.GradeType = gradeType;
        grade.Date = dto.Date ?? grade.Date;
        grade.ModifiedBy = changedBy;
        grade.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.Grades.UpdateAsync(grade);
        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.Grades.AddGradeChangeAsync(gradeId, previousValue, newGradeValue, changedBy);

        return await GetGradeWithHistoryAsync(gradeId);
    }

    public async Task<LecturerDetailDto?> GetLecturerProfileAsync(Guid lecturerId)
    {
        var lecturer = await _unitOfWork.Lecturers.FindByIdAsync(lecturerId);
        if (lecturer == null)
            return null;

        var courses = await _unitOfWork.Lecturers.GetCoursesByLecturerAsync(lecturerId);

        return new LecturerDetailDto
        {
            Id = lecturer.Id,
            FirstName = lecturer.FirstName,
            LastName = lecturer.LastName,
            Email = lecturer.Email.ToString(),
            Title = lecturer.Title,
            Faculty = lecturer.Faculty,
            Pesel = lecturer.Pesel?.ToString() ?? "N/A",
            TaughtCoursesCount = courses.Count()
        };
    }

    public async Task<LecturerDetailDto> CreateLecturerAsync(LecturerCreateDto dto)
    {
        var entity = _mapper.Map<Lecturer>(dto);
        if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
        
        var added = await _unitOfWork.Lecturers.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return await GetLecturerByIdAsync(added.Id) ?? throw new InvalidOperationException("Failed to retrieve created lecturer");
    }

    public async Task<LecturerDetailDto> UpdateLecturerAsync(Guid id, LecturerUpdateDto dto)
    {
        var existing = await _unitOfWork.Lecturers.FindByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Lecturer with id {id} not found");
        
        _mapper.Map(dto, existing);
        await _unitOfWork.Lecturers.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
        
        return await GetLecturerByIdAsync(id) ?? throw new InvalidOperationException("Failed to retrieve updated lecturer");
    }

    public async Task<LecturerDetailDto?> GetLecturerByIdAsync(Guid id)
    {
        var lecturer = await _unitOfWork.Lecturers.FindByIdAsync(id);
        if (lecturer == null) return null;
        
        var courses = await _unitOfWork.Lecturers.GetCoursesByLecturerAsync(id);
        
        return new LecturerDetailDto
        {
            Id = lecturer.Id,
            FirstName = lecturer.FirstName,
            LastName = lecturer.LastName,
            Email = lecturer.Email.ToString(),
            Title = lecturer.Title,
            Faculty = lecturer.Faculty,
            Pesel = lecturer.Pesel?.ToString() ?? "N/A",
            TaughtCoursesCount = courses.Count()
        };
    }

    private async Task<GradeWithHistoryDto> GetGradeWithHistoryAsync(Guid gradeId)
    {
        var grade = await _unitOfWork.Grades.FindByIdAsync(gradeId);
        if (grade == null)
            throw new InvalidOperationException($"Grade {gradeId} not found");

        var history = await _unitOfWork.Grades.GetChangeHistoryAsync(gradeId);

        return new GradeWithHistoryDto
        {
            Id = grade.Id,
            Value = (double)grade.GradeValue,
            Type = grade.GradeType.ToString(),
            Date = grade.Date,
            LecturerName = grade.Lecturer != null ? $"{grade.Lecturer.FirstName} {grade.Lecturer.LastName}" : null,
            CreatedBy = grade.CreatedBy,
            CreatedAt = grade.CreatedAt,
            ModifiedBy = grade.ModifiedBy,
            ModifiedAt = grade.ModifiedAt,
            ChangeHistory = history.Select(h => new GradeChangeHistoryDto
            {
                Id = h.Id,
                PreviousValue = h.PreviousValue != null ? (double)h.PreviousValue : null,
                NewValue = (double)h.NewValue,
                ChangedBy = h.ChangedBy,
                ChangedAt = h.ChangedAt
            }).ToList()
        };
    }
}





