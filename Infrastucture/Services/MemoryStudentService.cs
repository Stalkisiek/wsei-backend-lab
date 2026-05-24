using System;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Repositories;
using AutoMapper;

namespace Infrastucture.Services;

public class MemoryStudentService : CoreApp.Services.IStudentService
{
    private readonly IUniversityUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MemoryStudentService(IUniversityUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<StudentSummaryDto>> FindAllStudentsPagedAsync(int page, int pageSize)
    {
        var people = await _unitOfWork.Students.FindPagedAsync(page, pageSize);
        var items = people.Items.Select(p => _mapper.Map<StudentSummaryDto>(p)).ToList();
        return new PagedResult<StudentSummaryDto>(items, people.TotalCount, people.Page, people.PageSize);
    }

    public async Task<StudentDetailDto?> GetStudentByIdAsync(Guid id)
    {
        var s = await _unitOfWork.Students.FindByIdAsync(id);
        if (s == null) return null;
        return _mapper.Map<StudentDetailDto>(s);
    }

    public async Task<Student?> GetStudentById(Guid id)
    {
        var s = await _unitOfWork.Students.FindByIdAsync(id);
        return s as Student;
    }

    public async Task<StudentSummaryDto?> GetById(Guid id)
    {
        var s = await _unitOfWork.Students.FindByIdAsync(id);
        if (s == null) return null;
        return _mapper.Map<StudentSummaryDto>(s);
    }

    public async Task<StudentDetailDto> CreateStudentAsync(StudentCreateDto dto)
    {
        var entity = _mapper.Map<Student>(dto);
        var added = await _unitOfWork.Students.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentDetailDto>(added);
    }

    public async Task<Student> AddStudent(StudentCreateDto dto)
    {
        var entity = _mapper.Map<Student>(dto);
        var added = await _unitOfWork.Students.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return added;
    }

    public async Task<StudentDetailDto> UpdateStudentAsync(Guid id, StudentUpdateDto dto)
    {
        var existing = await _unitOfWork.Students.FindByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Student with id {id} not found");
        var student = (Student)existing;
        _mapper.Map(dto, student);
        var updated = await _unitOfWork.Students.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentDetailDto>(updated);
    }

    public async Task<Student> UpdateStudent(Guid id, StudentUpdateDto dto)
    {
        var existing = await _unitOfWork.Students.FindByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Student with id {id} not found");
        var student = (Student)existing;
        _mapper.Map(dto, student);
        var updated = await _unitOfWork.Students.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();
        return updated;
    }

    public async Task<Grade> AddGrade(Guid studentId, GradeDto gradeDto)
    {
        var existing = await _unitOfWork.Students.FindByIdAsync(studentId);
        if (existing == null) throw new KeyNotFoundException($"Student with id {studentId} not found");
        var student = (Student)existing;

        var course = await _unitOfWork.Courses.FindByIdAsync(gradeDto.CourseId);
        CoreApp.Models.Lecturer? lecturer = null;
        if (gradeDto.LecturerId.HasValue)
        {
            var l = await _unitOfWork.Lecturers.FindByIdAsync(gradeDto.LecturerId.Value);
            if (l == null)
            {
                throw new CoreApp.Exceptions.LecturerNotFoundException($"Lecturer with id={gradeDto.LecturerId.Value} not found!");
            }
            lecturer = l as CoreApp.Models.Lecturer;
        }

        CoreApp.Models.AcademicYear? academicYear = null;
        if (gradeDto.AcademicYearId.HasValue)
        {
            var y = await _unitOfWork.AcademicYears.FindByIdAsync(gradeDto.AcademicYearId.Value);
            academicYear = y as CoreApp.Models.AcademicYear;
        }

        // convert grade value (double) to enum GradeValue
        var allowedMap = new System.Collections.Generic.Dictionary<int, GradeValue>
        {
            [20] = GradeValue.Grade20,
            [30] = GradeValue.Grade30,
            [35] = GradeValue.Grade35,
            [40] = GradeValue.Grade40,
            [45] = GradeValue.Grade45,
            [50] = GradeValue.Grade50
        };
        var gv10 = (int)Math.Round(gradeDto.GradeValue * 10);
        if (!allowedMap.TryGetValue(gv10, out var gv)) throw new ArgumentException($"Invalid grade value: {gradeDto.GradeValue}");

        var grade = new Grade
        {
            Id = Guid.NewGuid(),
            Student = student,
            Course = course ?? new Course { Id = gradeDto.CourseId, Code = string.Empty, Name = string.Empty, Enrollments = new List<Student>() },
            Date = gradeDto.Date,
            GradeType = gradeDto.GradeType,
            Lecturer = lecturer,
            AcademicYear = academicYear,
            GradeValue = gv
        };

        var added = await _unitOfWork.Grades.AddAsync(grade);
        await _unitOfWork.SaveChangesAsync();
        return added;
    }

    public async Task<Grade> UpdateGrade(Guid studentId, Guid gradeId, GradeUpdateDto dto)
    {
        var existingGrade = await _unitOfWork.Grades.FindByIdAsync(gradeId);
        if (existingGrade == null) throw new KeyNotFoundException($"Grade with id {gradeId} not found");
        var grade = (Grade)existingGrade;
        if (grade.Student == null || grade.Student.Id != studentId) throw new KeyNotFoundException($"Grade with id {gradeId} for student {studentId} not found");

        var allowedMap = new System.Collections.Generic.Dictionary<int, GradeValue>
        {
            [20] = GradeValue.Grade20,
            [30] = GradeValue.Grade30,
            [35] = GradeValue.Grade35,
            [40] = GradeValue.Grade40,
            [45] = GradeValue.Grade45,
            [50] = GradeValue.Grade50
        };
        var gv10 = (int)Math.Round(dto.GradeValue * 10);
        if (!allowedMap.TryGetValue(gv10, out var gv)) throw new ArgumentException($"Invalid grade value: {dto.GradeValue}");

        grade.GradeValue = gv;
        grade.Date = dto.Date;
        grade.GradeType = dto.GradeType;

        var updated = await _unitOfWork.Grades.UpdateAsync(grade);
        await _unitOfWork.SaveChangesAsync();
        return updated;
    }

    public async Task<StudentDetailDto> UpdateStudentStatusAsync(Guid id, StudentStatus newStatus)
    {
        var updated = await _unitOfWork.Students.UpdateStatusAsync(id, newStatus);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentDetailDto>(updated);
    }

    public async Task<IEnumerable<GradeDto>> GetGradesAsync(Guid studentId)
    {
        var grades = await _unitOfWork.Grades.FindByStudentAsync(studentId);
        var items = grades.Select(g => new GradeDto
        {
            Id = g.Id,
            CourseId = g.Course?.Id ?? Guid.Empty,
            GradeValue = g.GradeValue.Value(),
            GradeType = g.GradeType,
            LecturerId = g.Lecturer?.Id,
            AcademicYearId = g.AcademicYear?.Id,
            Date = g.Date
        }).ToList();
        return items;
    }
}


