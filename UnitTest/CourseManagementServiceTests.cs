using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Repositories;
using CoreApp.Services;
using Infrastucture.Memory;
using Infrastucture.Repository;

namespace UnitTest;

public class CourseManagementServiceTests
{
    private static CourseManagementService CreateService(
        out ICourseRepository courseRepository,
        out IStudentRepository studentRepository,
        out ILecturerRepository lecturerRepository,
        out IAcademicYearRepository academicYearRepository,
        out IDegreeProgramRepository degreeProgramRepository,
        out IGradeRepository gradeRepository)
    {
        studentRepository = new MemoryStudentRepository();
        lecturerRepository = new MemoryLecturerRepository();
        gradeRepository = new MemoryGradeRepository();
        courseRepository = new MemoryCourseRepository();
        academicYearRepository = new MemoryAcademicYearRepository();
        degreeProgramRepository = new MemoryDegreeProgramRepository();

        var uow = new MemoryUniversityUnitOfWork(
            studentRepository,
            lecturerRepository,
            gradeRepository,
            courseRepository,
            academicYearRepository,
            degreeProgramRepository);

        return new CourseManagementService(uow);
    }

    [Fact]
    public async Task CreateCourseAsync_ShouldCreateCourseWithProgramAndYear()
    {
        var service = CreateService(out var courseRepository, out _, out _, out var yearRepository, out var programRepository, out _);

        var year = (await academicYears(yearRepository)).First();
        var program = (await degreePrograms(programRepository)).First();

        var dto = new CourseCreateDto
        {
            Code = "ALG-101",
            Name = "Algorytmy",
            EctsCredits = 6,
            CompletionType = CompletionType.Exam,
            Semester = Semester.Winter,
            DegreeProgramId = program.Id,
            AcademicYearId = year.Id
        };

        var created = await service.CreateCourseAsync(dto);

        var persisted = await courseRepository.FindByCodeAsync("ALG-101");
        Assert.NotNull(persisted);
        Assert.Equal("ALG-101", created.Code);
        Assert.Equal("Winter", created.Semester);
        Assert.Equal(program.Id, created.DegreeProgramId);
        Assert.Equal(year.Id, created.AcademicYearId);
    }

    [Fact]
    public async Task AssignLecturerAsync_ShouldSetLecturerOnCourse()
    {
        var service = CreateService(out var courseRepository, out _, out var lecturerRepository, out _, out _, out _);

        var course = await courseRepository.AddAsync(new Course
        {
            Id = Guid.NewGuid(),
            Code = "DB-201",
            Name = "Bazy Danych",
            EctsCredits = 5,
            CompletionType = CompletionType.Exam,
            Semester = Semester.Summer,
            Enrollments = new List<Student>()
        });

        var lecturer = (await lecturers(lecturerRepository)).First();

        var updated = await service.AssignLecturerAsync(course.Id, lecturer.Id);

        Assert.Equal(lecturer.Id, updated.LecturerId);
    }

    [Fact]
    public async Task EnrollAndUnenrollStudentAsync_ShouldUpdateEnrollmentCount()
    {
        var service = CreateService(out var courseRepository, out var studentRepository, out _, out _, out _, out _);

        var student = await studentRepository.AddAsync(new Student
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2026-7001",
            FirstName = "Jan",
            LastName = "Test",
            Email = EmailAddress.From("jan.test@student.local"),
            ProgramName = "INF-BSC",
            YearOfStudy = 1,
            EnrollmentYear = "2026",
            Status = StudentStatus.Active,
            Grades = new List<Grade>()
        });

        var course = await courseRepository.AddAsync(new Course
        {
            Id = Guid.NewGuid(),
            Code = "NET-101",
            Name = "Sieci",
            EctsCredits = 4,
            CompletionType = CompletionType.Exam,
            Semester = Semester.Winter,
            Enrollments = new List<Student>()
        });

        var enrolled = await service.EnrollStudentAsync(course.Id, student.Id);
        Assert.Equal(1, enrolled.EnrolledStudentsCount);

        var unenrolled = await service.UnenrollStudentAsync(course.Id, student.Id);
        Assert.Equal(0, unenrolled.EnrolledStudentsCount);
    }

    [Fact]
    public async Task GetCourseReportAsync_ShouldReturnPassRateStats()
    {
        var service = CreateService(out var courseRepository, out var studentRepository, out var lecturerRepository, out _, out _, out var gradeRepository);

        var lecturer = (await lecturers(lecturerRepository)).First();

        var studentPass = await studentRepository.AddAsync(new Student
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2026-7101",
            FirstName = "Pass",
            LastName = "Student",
            Email = EmailAddress.From("pass@student.local"),
            ProgramName = "INF-BSC",
            YearOfStudy = 2,
            EnrollmentYear = "2025",
            Status = StudentStatus.Active,
            Grades = new List<Grade>()
        });

        var studentFail = await studentRepository.AddAsync(new Student
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2026-7102",
            FirstName = "Fail",
            LastName = "Student",
            Email = EmailAddress.From("fail@student.local"),
            ProgramName = "INF-BSC",
            YearOfStudy = 2,
            EnrollmentYear = "2025",
            Status = StudentStatus.Active,
            Grades = new List<Grade>()
        });

        var course = await courseRepository.AddAsync(new Course
        {
            Id = Guid.NewGuid(),
            Code = "STAT-301",
            Name = "Statystyka",
            EctsCredits = 5,
            CompletionType = CompletionType.Exam,
            Semester = Semester.Summer,
            Lecturer = lecturer,
            Enrollments = new List<Student> { studentPass, studentFail }
        });

        await gradeRepository.AddAsync(new Grade
        {
            Id = Guid.NewGuid(),
            Student = studentPass,
            Course = course,
            Lecturer = lecturer,
            GradeValue = GradeValue.Grade40,
            GradeType = GradeType.Final,
            Date = DateTime.UtcNow
        });

        await gradeRepository.AddAsync(new Grade
        {
            Id = Guid.NewGuid(),
            Student = studentFail,
            Course = course,
            Lecturer = lecturer,
            GradeValue = GradeValue.Grade20,
            GradeType = GradeType.Final,
            Date = DateTime.UtcNow
        });

        var report = await service.GetCourseReportAsync(course.Id);

        Assert.NotNull(report);
        Assert.Equal(2, report!.EnrolledStudentsCount);
        Assert.Equal(2, report.GradedStudentsCount);
        Assert.Equal(1, report.PassedStudentsCount);
        Assert.Equal(1, report.FailedStudentsCount);
        Assert.Equal(50.0, report.PassRatePercent);
    }

    private static Task<IEnumerable<AcademicYear>> academicYears(IAcademicYearRepository repo)
    {
        return repo.FindAllAsync();
    }

    private static Task<IEnumerable<DegreeProgram>> degreePrograms(IDegreeProgramRepository repo)
    {
        return repo.FindAllAsync();
    }

    private static Task<IEnumerable<Lecturer>> lecturers(ILecturerRepository repo)
    {
        return repo.FindAllAsync();
    }
}

