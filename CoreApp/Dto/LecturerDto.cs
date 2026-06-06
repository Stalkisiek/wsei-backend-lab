namespace CoreApp.Dto;

public sealed record LecturerSummaryDto 
{
    public string Title    { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
}

public sealed record LecturerDetailDto: PersonDto
{
    public Guid Id { get; init; }
    public string Title     { get; init; } = string.Empty;
    public string Faculty   { get; init; } = string.Empty;
    public string Pesel { get; init; } = string.Empty;
    public int TaughtCoursesCount { get; init; }
}

public sealed record LecturerCreateDto: PersonCreateDto
{
    public string Title      { get; init; } = string.Empty;
    public string Faculty    { get; init; } = string.Empty;
}

public sealed record LecturerUpdateDto: PersonDto
{
    public string Title     { get; init; } = string.Empty;
    public string Faculty   { get; init; } = string.Empty;
}

public sealed record LecturerStudentDto
{
    public Guid Id { get; init; }
    public string StudentId { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Pesel { get; init; } = string.Empty;
    public int YearOfStudy { get; init; }
    public string ProgramName { get; init; } = string.Empty;
}

public sealed record LecturerGradeUpdateDto
{
    public double GradeValue { get; init; }
    public string GradeType { get; init; } = string.Empty;
    public DateTime? Date { get; init; }
}

public sealed record GradeWithHistoryDto
{
    public Guid Id { get; init; }
    public double Value { get; init; }
    public string Type { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string? LecturerName { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? ModifiedBy { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public List<GradeChangeHistoryDto> ChangeHistory { get; init; } = new();
}

public sealed record GradeChangeHistoryDto
{
    public Guid Id { get; init; }
    public double? PreviousValue { get; init; }
    public double NewValue { get; init; }
    public string ChangedBy { get; init; } = string.Empty;
    public DateTime ChangedAt { get; init; }
}

public sealed record LecturerCourseDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int EctsCredits { get; init; }
    public string CompletionType { get; init; } = string.Empty;
    public int EnrolledStudentsCount { get; init; }
}

