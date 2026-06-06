using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Repositories;

namespace CoreApp.Services;

public class CourseManagementService : ICourseManagementService
{
    private readonly IUniversityUnitOfWork _unitOfWork;

    public CourseManagementService(IUniversityUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CourseDetailDto> CreateCourseAsync(CourseCreateDto dto)
    {
        var existing = await _unitOfWork.Courses.FindByCodeAsync(dto.Code.Trim());
        if (existing != null)
            throw new ArgumentException($"Course code {dto.Code} already exists.");

        var degreeProgram = await _unitOfWork.DegreePrograms.FindByIdAsync(dto.DegreeProgramId)
            ?? throw new KeyNotFoundException($"Degree program {dto.DegreeProgramId} not found.");

        var academicYear = await _unitOfWork.AcademicYears.FindByIdAsync(dto.AcademicYearId)
            ?? throw new KeyNotFoundException($"Academic year {dto.AcademicYearId} not found.");

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            EctsCredits = dto.EctsCredits,
            CompletionType = dto.CompletionType,
            Semester = dto.Semester,
            DegreeProgram = degreeProgram,
            AcademicYear = academicYear,
            Enrollments = new List<Student>()
        };

        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        var detailed = await _unitOfWork.Courses.FindDetailedByIdAsync(course.Id) ?? course;
        return MapCourse(detailed);
    }

    public async Task<CourseDetailDto> AssignLecturerAsync(Guid courseId, Guid lecturerId)
    {
        var course = await _unitOfWork.Courses.FindDetailedByIdAsync(courseId)
            ?? throw new KeyNotFoundException($"Course {courseId} not found.");

        var lecturer = await _unitOfWork.Lecturers.FindByIdAsync(lecturerId)
            ?? throw new KeyNotFoundException($"Lecturer {lecturerId} not found.");

        course.Lecturer = lecturer;
        if (!lecturer.TaughtCorses.Any(c => c.Id == course.Id))
            lecturer.TaughtCorses.Add(course);

        await _unitOfWork.Courses.UpdateAsync(course);
        await _unitOfWork.SaveChangesAsync();

        var detailed = await _unitOfWork.Courses.FindDetailedByIdAsync(course.Id) ?? course;
        return MapCourse(detailed);
    }

    public async Task<CourseDetailDto> EnrollStudentAsync(Guid courseId, Guid studentId)
    {
        var course = await _unitOfWork.Courses.FindDetailedByIdAsync(courseId)
            ?? throw new KeyNotFoundException($"Course {courseId} not found.");

        var student = await _unitOfWork.Students.FindByIdAsync(studentId)
            ?? throw new KeyNotFoundException($"Student {studentId} not found.");

        if (!course.Enrollments.Any(s => s.Id == student.Id))
            course.Enrollments.Add(student);

        await _unitOfWork.Courses.UpdateAsync(course);
        await _unitOfWork.SaveChangesAsync();

        var detailed = await _unitOfWork.Courses.FindDetailedByIdAsync(course.Id) ?? course;
        return MapCourse(detailed);
    }

    public async Task<CourseDetailDto> UnenrollStudentAsync(Guid courseId, Guid studentId)
    {
        var course = await _unitOfWork.Courses.FindDetailedByIdAsync(courseId)
            ?? throw new KeyNotFoundException($"Course {courseId} not found.");

        var existing = course.Enrollments.FirstOrDefault(s => s.Id == studentId);
        if (existing != null)
            course.Enrollments.Remove(existing);

        await _unitOfWork.Courses.UpdateAsync(course);
        await _unitOfWork.SaveChangesAsync();

        var detailed = await _unitOfWork.Courses.FindDetailedByIdAsync(course.Id) ?? course;
        return MapCourse(detailed);
    }

    public async Task<CourseReportDto?> GetCourseReportAsync(Guid courseId)
    {
        var course = await _unitOfWork.Courses.FindDetailedByIdAsync(courseId);
        if (course == null)
            return null;

        var grades = (await _unitOfWork.Grades.FindByCourseAsync(courseId)).ToList();
        var gradedStudentIds = grades.Where(g => g.Student != null).Select(g => g.Student.Id).Distinct().ToList();
        var passedStudentIds = grades
            .Where(g => g.Student != null && (int)g.GradeValue >= (int)GradeValue.Grade30)
            .Select(g => g.Student!.Id)
            .Distinct()
            .ToList();

        var gradedCount = gradedStudentIds.Count;
        var passedCount = passedStudentIds.Count;
        var failedCount = Math.Max(0, gradedCount - passedCount);
        var passRate = gradedCount == 0 ? 0.0 : Math.Round((double)passedCount * 100.0 / gradedCount, 2);

        return new CourseReportDto
        {
            CourseId = course.Id,
            Code = course.Code,
            Name = course.Name,
            EnrolledStudentsCount = course.Enrollments.Count,
            GradedStudentsCount = gradedCount,
            PassedStudentsCount = passedCount,
            FailedStudentsCount = failedCount,
            PassRatePercent = passRate
        };
    }

    private static CourseDetailDto MapCourse(Course course)
    {
        return new CourseDetailDto
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name,
            EctsCredits = course.EctsCredits,
            CompletionType = course.CompletionType.ToString(),
            Semester = course.Semester.ToString(),
            DegreeProgramId = course.DegreeProgram?.Id ?? Guid.Empty,
            DegreeProgramCode = course.DegreeProgram?.Code ?? string.Empty,
            AcademicYearId = course.AcademicYear?.Id ?? Guid.Empty,
            AcademicYearName = course.AcademicYear?.Name ?? string.Empty,
            LecturerId = course.Lecturer?.Id,
            LecturerName = course.Lecturer != null ? $"{course.Lecturer.FirstName} {course.Lecturer.LastName}" : null,
            EnrolledStudentsCount = course.Enrollments?.Count ?? 0
        };
    }
}

