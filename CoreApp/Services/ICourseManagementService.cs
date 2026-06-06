using CoreApp.Dto;

namespace CoreApp.Services;

public interface ICourseManagementService
{
    Task<CourseDetailDto> CreateCourseAsync(CourseCreateDto dto);
    Task<CourseDetailDto> AssignLecturerAsync(Guid courseId, Guid lecturerId);
    Task<CourseDetailDto> EnrollStudentAsync(Guid courseId, Guid studentId);
    Task<CourseDetailDto> UnenrollStudentAsync(Guid courseId, Guid studentId);
    Task<CourseReportDto?> GetCourseReportAsync(Guid courseId);
}

