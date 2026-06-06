using System.Security.Claims;
using CoreApp.Dto;
using CoreApp.Repositories;
using CoreApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILecturerRepository _lecturerRepository;

    public AuthController(IAuthService authService, ILecturerRepository lecturerRepository)
    {
        _authService = authService;
        _lecturerRepository = lecturerRepository;
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
    
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
    
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto dto)
    {
        await _authService.RevokeTokenAsync(dto.RefreshToken);
        return NoContent();
    }
    
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me()
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        var firstName = User.FindFirstValue(ClaimTypes.GivenName);
        var lastName = User.FindFirstValue(ClaimTypes.Surname);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var isLecturer = roles.Any(r => string.Equals(r, "Lecturer", StringComparison.OrdinalIgnoreCase));
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (isLecturer)
        {
            var lecturer = (await _lecturerRepository.FindAllAsync())
                .FirstOrDefault(l =>
                    string.Equals(l.Email.ToString(), email, StringComparison.OrdinalIgnoreCase) ||
                    (firstName != null && lastName != null &&
                     string.Equals(l.FirstName, firstName, StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(l.LastName, lastName, StringComparison.OrdinalIgnoreCase)));

            if (lecturer != null)
            {
                userId = lecturer.Id.ToString();
            }
        }

        var user = new UserDto
        {
            Id = userId,
            Email = email,
            FirstName = User.FindFirstValue(ClaimTypes.GivenName)!,
            LastName = User.FindFirstValue(ClaimTypes.Surname)!,
            Department = User.FindFirstValue("department")!,
            Roles = roles
        };

        return Ok(user);
    }
}

