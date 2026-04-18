using CoreApp.Models;

namespace CoreApp.Dto;

public sealed record StudentSummaryDto : PersonDto
{
    public string StudentId { get; init; } = string.Empty;
    public string ProgramName { get; init; } = string.Empty;
    public int YearOfStudy { get; init; }
    public StudentStatus Status { get; init; }

    public static StudentSummaryDto FromEntity(Student student)
    {
        return new StudentSummaryDto()
        {
            StudentId = student.StudentId.ToString(),
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            ProgramName = student.ProgramName,
            YearOfStudy = student.YearOfStudy,
            Status = student.Status
        };
    }
}

public sealed record StudentDetailDto : PersonDto
{
    public string StudentId { get; init; } = string.Empty;
    public string ProgramCode { get; init; } = string.Empty;
    public string ProgramName { get; init; } = string.Empty;
    public string EnrollmentYear { get; init; } = string.Empty;
    public int YearOfStudy { get; init; }
    public StudentStatus Status { get; init; }
    public double GradePointAverage { get; init; }
    public int TotalEctsEarned { get; init; }
    public bool IsEligibleForDiploma { get; init; }
}

public sealed record StudentCreateDto : PersonCreateDto
{
    public string StudentId { get; init; } = string.Empty;
    public int YearOfStudy { get; init; } = 1;
    public string ProgramCode { get; init; } = string.Empty;
    public int EnrollmentYearFrom { get; init; }

    public static Student ToEntity(StudentCreateDto dto)
    {
        return new Student
        {
            StudentId = Guid.Parse(dto.StudentId),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            YearOfStudy = dto.YearOfStudy,
            ProgramName = dto.ProgramCode,
            Status = StudentStatus.Active
        };
    }
}

public sealed record StudentUpdateDto : PersonDto
{
    public int YearOfStudy { get; init; }
    public StudentStatus Status { get; init; }
    public string ProgramCode { get; init; } = string.Empty;
}