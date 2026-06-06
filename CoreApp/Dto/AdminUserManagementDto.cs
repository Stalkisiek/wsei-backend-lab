using CoreApp.Models;

namespace CoreApp.Dto;

public sealed record AdminCreateUserDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public IEnumerable<string> Roles { get; init; } = [];
    public IEnumerable<string> Permissions { get; init; } = [];
}

public sealed record AdminUserRolesDto
{
    public IEnumerable<string> Roles { get; init; } = [];
}

public sealed record AdminUserPermissionsDto
{
    public IEnumerable<string> Permissions { get; init; } = [];
}

public sealed record AdminUserStatusDto
{
    public SystemUserStatus Status { get; init; }
}

public sealed record AdminUserTransferDto
{
    public string Department { get; init; } = string.Empty;
}

public sealed record AdminUserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public SystemUserStatus Status { get; init; }
    public IEnumerable<string> Roles { get; init; } = [];
    public IEnumerable<string> Permissions { get; init; } = [];
}

