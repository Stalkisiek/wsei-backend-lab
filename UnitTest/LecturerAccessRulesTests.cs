using AutoMapper;
using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Services;
using Infrastucture.Memory;
using Infrastucture.Repository;
using Microsoft.Extensions.Logging.Abstractions;

namespace UnitTest;

public class LecturerAccessRulesTests
{
    private static LecturerService CreateService(
        out MemoryStudentRepository studentRepository,
        out MemoryLecturerRepository lecturerRepository,
        out MemoryGradeRepository gradeRepository,
        out MemoryCourseRepository courseRepository)
    {
        studentRepository = new MemoryStudentRepository();
        lecturerRepository = new MemoryLecturerRepository();
        gradeRepository = new MemoryGradeRepository();
        courseRepository = new MemoryCourseRepository();
        var yearRepository = new MemoryAcademicYearRepository();
        var degreeProgramRepository = new MemoryDegreeProgramRepository();

        var unitOfWork = new MemoryUniversityUnitOfWork(
            studentRepository,
            lecturerRepository,
            gradeRepository,
            courseRepository,
            yearRepository,
            degreeProgramRepository);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
            cfg.AddProfile(new CoreApp.Mapper.StudentsMappingProfile());
        }, NullLoggerFactory.Instance);

        return new LecturerService(unitOfWork, mapperConfig.CreateMapper());
    }

    [Fact]
    public async Task GetStudentsByCourseAsync_ShouldReturnStudents_ForUnassignedLecturerRead()
    {
        var service = CreateService(out var studentRepository, out var lecturerRepository, out _, out var courseRepository);

        var lecturers = (await ((CoreApp.Repositories.ILecturerRepository)lecturerRepository).FindAllAsync()).ToList();
        var assignedLecturer = lecturers[0];
        var unassignedLecturer = lecturers[1];

        var student = new Student
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2026-3001",
            FirstName = "Read",
            LastName = "Only",
            Email = EmailAddress.From("readonly@student.local"),
            ProgramName = "INF-BSC",
            YearOfStudy = 1,
            EnrollmentYear = "2026",
            Status = StudentStatus.Active,
            Grades = new List<Grade>()
        };

        await studentRepository.AddAsync(student);

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Code = "RO-101",
            Name = "Read Access",
            EctsCredits = 3,
            CompletionType = CompletionType.Exam,
            Lecturer = assignedLecturer,
            Enrollments = new List<Student> { student }
        };

        assignedLecturer.TaughtCorses.Add(course);
        await lecturerRepository.UpdateAsync(assignedLecturer);
        await courseRepository.AddAsync(course);

        var result = (await service.GetStudentsByCourseAsync(unassignedLecturer.Id, course.Id)).ToList();

        Assert.Single(result);
        Assert.Equal(student.Id, result[0].Id);
        Assert.Equal("readonly@student.local", result[0].Email);
    }

    [Fact]
    public async Task GetStudentGradesAsync_ShouldReturnGrades_ForUnassignedLecturerRead()
    {
        var service = CreateService(out var studentRepository, out var lecturerRepository, out var gradeRepository, out var courseRepository);

        var assignedLecturer = (await ((CoreApp.Repositories.ILecturerRepository)lecturerRepository).FindAllAsync()).First();

        var student = new Student
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2026-3002",
            FirstName = "Grade",
            LastName = "Reader",
            Email = EmailAddress.From("gradereader@student.local"),
            ProgramName = "INF-BSC",
            YearOfStudy = 2,
            EnrollmentYear = "2025",
            Status = StudentStatus.Active,
            Grades = new List<Grade>()
        };

        await studentRepository.AddAsync(student);

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Code = "GR-101",
            Name = "Grades Read",
            EctsCredits = 4,
            CompletionType = CompletionType.Exam,
            Lecturer = assignedLecturer,
            Enrollments = new List<Student> { student }
        };

        await courseRepository.AddAsync(course);

        var grade = new Grade
        {
            Id = Guid.NewGuid(),
            Student = student,
            Course = course,
            Lecturer = assignedLecturer,
            GradeValue = GradeValue.Grade45,
            GradeType = GradeType.Final,
            Date = DateTime.UtcNow,
            CreatedBy = "lecturer1"
        };

        await gradeRepository.AddAsync(grade);

        var result = (await service.GetStudentGradesAsync(Guid.NewGuid(), student.Id, course.Id)).ToList();

        Assert.Single(result);
        Assert.Equal((double)GradeValue.Grade45, result[0].Value);
    }

    [Fact]
    public async Task AddGradeAsync_ShouldThrowUnauthorized_ForUnassignedLecturer()
    {
        var service = CreateService(out var studentRepository, out var lecturerRepository, out _, out var courseRepository);

        var lecturers = (await ((CoreApp.Repositories.ILecturerRepository)lecturerRepository).FindAllAsync()).ToList();
        var assignedLecturer = lecturers[0];
        var unassignedLecturer = lecturers[1];

        var student = new Student
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2026-3003",
            FirstName = "Write",
            LastName = "Blocked",
            Email = EmailAddress.From("writeblocked@student.local"),
            ProgramName = "INF-BSC",
            YearOfStudy = 2,
            EnrollmentYear = "2025",
            Status = StudentStatus.Active,
            Grades = new List<Grade>()
        };

        await studentRepository.AddAsync(student);

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Code = "WR-101",
            Name = "Write Access",
            EctsCredits = 4,
            CompletionType = CompletionType.Exam,
            Lecturer = assignedLecturer,
            Enrollments = new List<Student> { student }
        };

        assignedLecturer.TaughtCorses.Add(course);
        await lecturerRepository.UpdateAsync(assignedLecturer);
        await courseRepository.AddAsync(course);

        var dto = new LecturerGradeUpdateDto
        {
            GradeValue = 4.0,
            GradeType = "Final",
            Date = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.AddGradeAsync(unassignedLecturer.Id, student.Id, course.Id, dto, "lecturer2"));
    }
}


