using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Services;
using Infrastucture.EntityFramework.Context;
using Infrastucture.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastucture.Security;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtOptions;

    public AuthService(
        UserManager<AppUser> userManager,
        AppDbContext context,
        JwtSettings jwtOptions)
    {
        _userManager = userManager;
        _context = context;
        _jwtOptions = jwtOptions;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new Exception("Nieprawidłowy email lub hasło.");

        if (await _userManager.IsLockedOutAsync(user))
            throw new Exception("Konto jest zablokowane.");

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            await _userManager.AccessFailedAsync(user);
            throw new Exception("Nieprawidłowy email lub hasło.");
        }

        if (user.Status != SystemUserStatus.Active)
            throw new Exception("Konto jest nieaktywne.");

        await _userManager.ResetAccessFailedCountAsync(user);
        await _userManager.UpdateAsync(user);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var principal = GetPrincipalFromExpiredToken(dto.AccessToken);

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("Nieprawidłowy token.");

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new Exception("Użytkownik nie istnieje.");

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t =>
                t.Token == dto.RefreshToken &&
                t.UserId == userId)
            ?? throw new Exception("Nieprawidłowy refresh token.");

        if (!refreshToken.IsActive)
            throw new Exception("Refresh token wygasł lub został odwołany.");

        var newResponse = await GenerateAuthResponseAsync(user);

        refreshToken.Revoke(newResponse.RefreshToken);
        await _context.SaveChangesAsync();

        return newResponse;
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken)
            ?? throw new Exception(nameof(RefreshToken) + " nie istnieje: " + refreshToken);

        if (!token.IsActive)
            throw new Exception("Token jest już nieaktywny.");

        token.Revoke();
        await _context.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = GenerateAccessToken(user, roles);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);
        var lecturers = await _context.Lecturers
            .AsNoTracking()
            .ToListAsync();

        var lecturer = lecturers.FirstOrDefault(l =>
            string.Equals(l.Email, user.Email, StringComparison.OrdinalIgnoreCase) ||
            (string.Equals(l.FirstName, user.FirstName, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(l.LastName, user.LastName, StringComparison.OrdinalIgnoreCase)));

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
            User = new UserDto
            {
                Id = lecturer?.Id.ToString() ?? user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Status = user.Status,
                Email = user.Email,
                Department = user.Department,
                Roles = roles
            }
        };
    }

    private string GenerateAccessToken(AppUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new("department", user.Department),
            new("status", user.Status.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var credentials = new SigningCredentials(
            _jwtOptions.GetSymmetricKey(),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(string userId)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync();

        foreach (var token in activeTokens)
            token.Revoke();

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = _jwtOptions.GetSymmetricKey()
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(accessToken, parameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Nieprawidłowy token.");
        }

        return principal;
    }
}

