using System;
using System.Threading.Tasks;
using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Repositories;

namespace CoreApp.Services;

public interface IStudentService
{
    Task<PagedResult<StudentSummaryDto>> FindAllStudentsPagedAsync(int page, int pageSize);
    Task<StudentDetailDto?> GetStudentByIdAsync(Guid id);
    Task<StudentDetailDto> CreateStudentAsync(StudentCreateDto dto);
    Task<StudentDetailDto> UpdateStudentAsync(Guid id, StudentUpdateDto dto);
    Task<StudentDetailDto> UpdateStudentStatusAsync(Guid id, StudentStatus newStatus);

    Task<Grade> AddGrade(Guid studentId, GradeDto gradeDto);
    Task<IEnumerable<GradeDto>> GetGradesAsync(Guid studentId);
    Task<Grade> UpdateGrade(Guid studentId, Guid gradeId, GradeUpdateDto dto);
    Task<StudentSummaryDto?> GetById(Guid id);

    Task<Student> AddStudent(StudentCreateDto dto);
    Task<Student> UpdateStudent(Guid id, StudentUpdateDto dto);
    Task<Student?> GetStudentById(Guid id);
}

