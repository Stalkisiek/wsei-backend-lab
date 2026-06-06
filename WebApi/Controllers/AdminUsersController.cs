using CoreApp.Authorization;
using CoreApp.Dto;
using CoreApp.Models;
using Infrastucture.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/admin/users")]
[Authorize(Policy = nameof(AppPolicies.Administrator))]
public class AdminUsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public AdminUsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users.ToList();
        var result = new List<AdminUserDto>();

        foreach (var user in users)
        {
            result.Add(await MapAsync(user));
        }

        return Ok(result.OrderBy(u => u.Email));
    }

    [HttpGet("roles")]
    public IActionResult GetAllRoles()
    {
        var roles = _roleManager.Roles.Select(r => r.Name!).OrderBy(r => r).ToList();
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { error = "Email and password are required" });

        var existing = await _userManager.FindByEmailAsync(dto.Email.Trim());
        if (existing != null)
            return Conflict(new { error = "User with this email already exists" });

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = dto.Email.Trim(),
            Email = dto.Email.Trim(),
            NormalizedEmail = dto.Email.Trim().ToUpperInvariant(),
            NormalizedUserName = dto.Email.Trim().ToUpperInvariant(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            FullName = $"{dto.FirstName.Trim()} {dto.LastName.Trim()}".Trim(),
            Department = string.IsNullOrWhiteSpace(dto.Department) ? "General" : dto.Department.Trim(),
            Status = SystemUserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var created = await _userManager.CreateAsync(user, dto.Password);
        if (!created.Succeeded)
            return BadRequest(new { errors = created.Errors.Select(e => e.Description) });

        var roles = dto.Roles.Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new AppRole(role));
        }

        if (roles.Count > 0)
        {
            var roleResult = await _userManager.AddToRolesAsync(user, roles);
            if (!roleResult.Succeeded)
                return BadRequest(new { errors = roleResult.Errors.Select(e => e.Description) });
        }

        foreach (var permission in dto.Permissions.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("permission", permission));
        }

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, await MapAsync(user));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { error = "User not found" });

        return Ok(await MapAsync(user));
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] AdminUserStatusDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { error = "User not found" });

        user.Status = dto.Status;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(await MapAsync(user));
    }

    [HttpPost("{id}/block")]
    public async Task<IActionResult> BlockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { error = "User not found" });

        user.Status = SystemUserStatus.Locked;
        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(await MapAsync(user));
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { error = "User not found" });

        user.Status = SystemUserStatus.Inactive;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(await MapAsync(user));
    }

    [HttpPost("{id}/transfer")]
    public async Task<IActionResult> TransferUser(string id, [FromBody] AdminUserTransferDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { error = "User not found" });

        if (string.IsNullOrWhiteSpace(dto.Department))
            return BadRequest(new { error = "Department is required" });

        user.Department = dto.Department.Trim();
        user.Status = SystemUserStatus.Inactive;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(await MapAsync(user));
    }

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRoles(string id, [FromBody] AdminUserRolesDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { error = "User not found" });

        var roles = dto.Roles.Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new AppRole(role));
        }

        var result = await _userManager.AddToRolesAsync(user, roles);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(await MapAsync(user));
    }

    [HttpDelete("{id}/roles/{role}")]
    public async Task<IActionResult> RevokeRole(string id, string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { error = "User not found" });

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(await MapAsync(user));
    }

    [HttpPost("{id}/permissions")]
    public async Task<IActionResult> AssignPermissions(string id, [FromBody] AdminUserPermissionsDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { error = "User not found" });

        var existingClaims = await _userManager.GetClaimsAsync(user);
        var existingPermissions = existingClaims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var permission in dto.Permissions.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!existingPermissions.Contains(permission))
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("permission", permission));
        }

        return Ok(await MapAsync(user));
    }

    [HttpDelete("{id}/permissions/{permission}")]
    public async Task<IActionResult> RevokePermission(string id, string permission)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { error = "User not found" });

        var claims = await _userManager.GetClaimsAsync(user);
        var permissionClaims = claims.Where(c => c.Type == "permission" && string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var claim in permissionClaims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        return Ok(await MapAsync(user));
    }

    private async Task<AdminUserDto> MapAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var permissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(v => v);

        return new AdminUserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Department = user.Department,
            Status = user.Status,
            Roles = roles.OrderBy(x => x),
            Permissions = permissions
        };
    }
}

