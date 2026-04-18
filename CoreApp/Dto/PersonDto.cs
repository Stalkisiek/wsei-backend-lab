namespace CoreApp.Dto;

public abstract record PersonDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName  { get; init; } = string.Empty;
    public string Email     { get; init; } = string.Empty;
}

public abstract record PersonCreateDto
{
    public string FirstName  { get; init; } = string.Empty;
    public string LastName   { get; init; } = string.Empty;
    public string NationalId { get; init; } = string.Empty;
    public string Email      { get; init; } = string.Empty;
}