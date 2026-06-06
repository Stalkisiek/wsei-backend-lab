using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Services;
using Infrastucture.Memory;
using Infrastucture.Repository;
using Xunit;

namespace UnitTest;

public class DegreeProgramServiceTests
{
    private static DegreeProgramService CreateService(
        out MemoryStudentRepository students,
        out MemoryDegreeProgramRepository programs)
    {
        students = new MemoryStudentRepository();
        var lecturers = new MemoryLecturerRepository();
        var grades = new MemoryGradeRepository();
        var courses = new MemoryCourseRepository();
        var years = new MemoryAcademicYearRepository();
        programs = new MemoryDegreeProgramRepository();

        var uow = new MemoryUniversityUnitOfWork(students, lecturers, grades, courses, years, programs);
        return new DegreeProgramService(uow);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewProgram()
    {
        var service = CreateService(out _, out _);

        var dto = new DegreeProgramCreateDto
        {
            Code = "MAT-MSC",
            Name = "Matematyka",
            DegreeType = DegreeType.Master,
            Faculty = "Wydzial Matematyki",
            DurationYears = 2,
            MinEctsForDiploma = 120
        };

        var created = await service.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("MAT-MSC", created.Code);
        Assert.Equal(DegreeType.Master, created.DegreeType);
        Assert.Equal(120, created.MinEctsForDiploma);
    }

    [Fact]
    public async Task GetReportAsync_ShouldReturnActiveAndGraduateCounts()
    {
        var service = CreateService(out var students, out _);

        var created = await service.CreateAsync(new DegreeProgramCreateDto
        {
            Code = "BIO-BSC",
            Name = "Biologia",
            DegreeType = DegreeType.Bachelor,
            Faculty = "Wydzial Biologii",
            DurationYears = 3,
            MinEctsForDiploma = 180
        });

        var program = new DegreeProgram
        {
            Id = created.Id,
            Code = created.Code,
            Name = created.Name,
            DegreeType = created.DegreeType,
            Faculty = created.Faculty,
            DurationYears = created.DurationYears,
            MinEctsForDiploma = created.MinEctsForDiploma,
            Courses = new List<Course>()
        };

        await students.AddAsync(new Student
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2026-1001",
            FirstName = "A",
            LastName = "A",
            Email = EmailAddress.From("a@wsei.local"),
            ProgramName = program.Code,
            DegreeProgram = program,
            YearOfStudy = 1,
            EnrollmentYear = "2026",
            Status = StudentStatus.Active,
            Grades = new List<Grade>()
        });

        await students.AddAsync(new Student
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2026-1002",
            FirstName = "B",
            LastName = "B",
            Email = EmailAddress.From("b@wsei.local"),
            ProgramName = program.Code,
            DegreeProgram = program,
            YearOfStudy = 3,
            EnrollmentYear = "2024",
            Status = StudentStatus.Graduate,
            Grades = new List<Grade>()
        });

        await students.AddAsync(new Student
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2026-1003",
            FirstName = "C",
            LastName = "C",
            Email = EmailAddress.From("c@wsei.local"),
            ProgramName = program.Code,
            DegreeProgram = program,
            YearOfStudy = 2,
            EnrollmentYear = "2025",
            Status = StudentStatus.OnLeave,
            Grades = new List<Grade>()
        });

        var report = await service.GetReportAsync(created.Id);

        Assert.NotNull(report);
        Assert.Equal("BIO-BSC", report!.Code);
        Assert.Equal(1, report.ActiveStudentsCount);
        Assert.Equal(1, report.GraduatesCount);
    }
}

