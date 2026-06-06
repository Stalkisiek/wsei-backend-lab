using CoreApp.Models;

namespace CoreApp.Dto;

public sealed record UpdateStatusDto
{
    public StudentStatus Status { get; init; }
}

