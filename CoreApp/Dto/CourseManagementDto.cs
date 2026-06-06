using CoreApp.Models;

namespace CoreApp.Dto;

public sealed record CourseCreateDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int EctsCredits { get; init; }
    public CompletionType CompletionType { get; init; }
    public Semester Semester { get; init; }
    public Guid DegreeProgramId { get; init; }
    public Guid AcademicYearId { get; init; }
}

public sealed record CourseDetailDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int EctsCredits { get; init; }
    public string CompletionType { get; init; } = string.Empty;
    public string Semester { get; init; } = string.Empty;
    public Guid DegreeProgramId { get; init; }
    public string DegreeProgramCode { get; init; } = string.Empty;
    public Guid AcademicYearId { get; init; }
    public string AcademicYearName { get; init; } = string.Empty;
    public Guid? LecturerId { get; init; }
    public string? LecturerName { get; init; }
    public int EnrolledStudentsCount { get; init; }
}

public sealed record CourseReportDto
{
    public Guid CourseId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int EnrolledStudentsCount { get; init; }
    public int GradedStudentsCount { get; init; }
    public int PassedStudentsCount { get; init; }
    public int FailedStudentsCount { get; init; }
    public double PassRatePercent { get; init; }
}

