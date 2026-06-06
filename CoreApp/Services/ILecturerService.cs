using CoreApp.Dto;
using CoreApp.Models;

namespace CoreApp.Services;

public interface ILecturerService
{
    Task<IEnumerable<LecturerCourseDto>> GetCoursesByLecturerAsync(Guid lecturerId);
    
    Task<IEnumerable<LecturerStudentDto>> GetStudentsByCourseAsync(Guid lecturerId, Guid courseId);
    
    Task<IEnumerable<GradeWithHistoryDto>> GetStudentGradesAsync(Guid lecturerId, Guid studentId, Guid courseId);
    
    Task<GradeWithHistoryDto> AddGradeAsync(Guid lecturerId, Guid studentId, Guid courseId, LecturerGradeUpdateDto dto, string changedBy);
    
    Task<GradeWithHistoryDto> UpdateGradeAsync(Guid lecturerId, Guid gradeId, LecturerGradeUpdateDto dto, string changedBy);
    
    Task<LecturerDetailDto?> GetLecturerProfileAsync(Guid lecturerId);

    Task<LecturerDetailDto> CreateLecturerAsync(LecturerCreateDto dto);

    Task<LecturerDetailDto> UpdateLecturerAsync(Guid id, LecturerUpdateDto dto);

    Task<LecturerDetailDto?> GetLecturerByIdAsync(Guid id);
}

