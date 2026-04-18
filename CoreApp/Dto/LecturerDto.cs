namespace CoreApp.Dto;

public sealed record LecturerSummaryDto 
{
    public string Title    { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
}

public sealed record LecturerDetailDto: PersonDto
{
    public string Title     { get; init; } = string.Empty;
    public string Faculty   { get; init; } = string.Empty;
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