using System;
using Microsoft.AspNetCore.Identity;
using CoreApp.Models;

namespace Infrastucture.EntityFramework.Entities;

public class AppRole : IdentityRole
{
    public string? Description { get; set; }

    public AppRole() { }
    public AppRole(string roleName, string? description = null)
        : base(roleName)
    {
        Description = description;
    }
}

public class AppUser : IdentityUser, ISystemUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Department { get; set; }
    public required SystemUserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }

    public void Activate()
    {
        if (Status == SystemUserStatus.Inactive)
        {
            Status = SystemUserStatus.Active;
            DeactivatedAt = null;
        }
    }

    public void Deactivate(DateTime now)
    {
        if (Status == SystemUserStatus.Active)
        {
            Status = SystemUserStatus.Inactive;
            DeactivatedAt = now;
        }
    }
}

