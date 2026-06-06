namespace CoreApp.Dto;

using CoreApp.Models;

public record LoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record UserDto
{
    public string Id { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public SystemUserStatus Status { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public IEnumerable<string> Roles { get; init; } = [];
}

public record AuthResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}

public record RefreshTokenDto(
    string AccessToken,
    string RefreshToken
);

public record RevokeTokenDto(
    string RefreshToken
);


