using CoreApp.Models;

namespace CoreApp.Dto;

public sealed record DegreeProgramCreateDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DegreeType DegreeType { get; init; }
    public string Faculty { get; init; } = string.Empty;
    public int DurationYears { get; init; }
    public int MinEctsForDiploma { get; init; }
}

public sealed record DegreeProgramDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DegreeType DegreeType { get; init; }
    public string Faculty { get; init; } = string.Empty;
    public int DurationYears { get; init; }
    public int MinEctsForDiploma { get; init; }
}

public sealed record DegreeProgramReportDto
{
    public Guid DegreeProgramId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int ActiveStudentsCount { get; init; }
    public int GraduatesCount { get; init; }
}

