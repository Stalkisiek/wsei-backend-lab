using Xunit;
using CoreApp.Dto;
using CoreApp.Models;

namespace UnitTest;

public class DeanOfficeApiTests
{
    [Fact]
    public void StudentUpdateDtoCanUpdateProgramAndYear()
    {
        var updateDto = new StudentUpdateDto
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Email = "jan.k@example.com",
            ProgramCode = "Informatyka",
            YearOfStudy = 2,
            Status = StudentStatus.Active
        };

        Assert.Equal("Informatyka", updateDto.ProgramCode);
        Assert.Equal(2, updateDto.YearOfStudy);
        Assert.Equal(StudentStatus.Active, updateDto.Status);
    }

    [Fact]
    public void UpdateStatusDtoHoldsStudentStatus()
    {
        var statusDto = new UpdateStatusDto
        {
            Status = StudentStatus.OnLeave
        };

        Assert.Equal(StudentStatus.OnLeave, statusDto.Status);
    }

    [Fact]
    public void LecturerCreateDtoCanBeUsedForRegistration()
    {
        var createDto = new LecturerCreateDto
        {
            Title = "Dr",
            Faculty = "Wydzial Informatyki",
            FirstName = "Piotr",
            LastName = "Nowak",
            Email = "piotr.nowak@university.edu"
        };

        Assert.Equal("Dr", createDto.Title);
        Assert.Equal("Wydzial Informatyki", createDto.Faculty);
        Assert.NotEmpty(createDto.FirstName);
        Assert.NotEmpty(createDto.LastName);
    }

    [Fact]
    public void LecturerUpdateDtoCanUpdateData()
    {
        var updateDto = new LecturerUpdateDto
        {
            Title = "Prof.",
            Faculty = "Wydzial Fizyki",
            FirstName = "Maria",
            LastName = "Kowalska",
            Email = "maria.k@university.edu"
        };

        Assert.Equal("Prof.", updateDto.Title);
        Assert.Equal("Wydzial Fizyki", updateDto.Faculty);
        Assert.NotEmpty(updateDto.Email);
    }

    [Fact]
    public void LecturerDetailDtoContainsAllRequiredInfo()
    {
        var detailDto = new LecturerDetailDto
        {
            Id = Guid.NewGuid(),
            Title = "Dr",
            Faculty = "Wydzial Matematyki",
            FirstName = "Anna",
            LastName = "Lewandowska",
            Email = "anna.l@university.edu",
            Pesel = "98765432101",
            TaughtCoursesCount = 3
        };

        Assert.NotEqual(Guid.Empty, detailDto.Id);
        Assert.Equal("Dr", detailDto.Title);
        Assert.NotEmpty(detailDto.Faculty);
        Assert.Equal(3, detailDto.TaughtCoursesCount);
    }

    [Fact]
    public void StudentDetailDtoContainsEnrollmentInfo()
    {
        var detailDto = new StudentDetailDto
        {
            Id = Guid.NewGuid(),
            StudentId = "ALB-2025-0001",
            FirstName = "Tomasz",
            LastName = "Wisniewski",
            Email = "tomasz.w@student.edu",
            ProgramName = "Informatyka",
            EnrollmentYear = "2023",
            YearOfStudy = 2,
            Status = StudentStatus.Active,
            ProgramCode = "INF",
            GradePointAverage = 3.8,
            TotalEctsEarned = 60,
            IsEligibleForDiploma = false
        };

        Assert.NotEqual(Guid.Empty, detailDto.Id);
        Assert.Equal("INF", detailDto.ProgramCode);
        Assert.Equal(2, detailDto.YearOfStudy);
        Assert.Equal(60, detailDto.TotalEctsEarned);
    }
}


