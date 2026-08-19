using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CRNProductApi.Application.DTOs.Auth;
using CRNProductApi.Application.Interfaces;
using CRNProductApi.Domain.Entities;
using CRNProductApi.Domain.Exceptions;
using CRNProductApi.Infrastructure.Repositories;

namespace CRNProductApi.Application.Services;

//Auth
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<long>> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IRepository<RefreshToken> _refreshTokenRepo;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<long>> roleManager,
        ITokenService tokenService,
        IRepository<RefreshToken> refreshTokenRepo)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            throw new ConflictException("User with this email already exists.");
        }

        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ArgumentException($"Registration failed: {errors}");
        }

        var roleName = string.Equals(dto.Role, UserRoles.Admin, StringComparison.OrdinalIgnoreCase)
            ? UserRoles.Admin
            : UserRoles.User;

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<long>(roleName));
        }

        await _userManager.AddToRoleAsync(user, roleName);

        var roles = await _userManager.GetRolesAsync(user);
        var authResponse = await _tokenService.GenerateTokensAsync(user, roles);

        return authResponse;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var authResponse = await _tokenService.GenerateTokensAsync(user, roles);

        return authResponse;
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, CancellationToken cancellationToken = default)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
        if (principal == null)
        {
            throw new UnauthorizedAccessException("Invalid access token.");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid token claims.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        var storedToken = _refreshTokenRepo.Table
            .FirstOrDefault(r => r.Token == dto.RefreshToken && r.UserId == userId);

        if (storedToken == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        if (storedToken.IsRevoked || storedToken.IsUsed)
        {
            throw new UnauthorizedAccessException("Refresh token has been revoked or used.");
        }

        if (storedToken.ExpiryDate < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token has expired.");
        }

        storedToken.IsUsed = true;
        await _refreshTokenRepo.UpdateAsync(storedToken);

        var roles = await _userManager.GetRolesAsync(user);
        var newAuthResponse = await _tokenService.GenerateTokensAsync(user, roles);

        storedToken.ReplacedByToken = newAuthResponse.RefreshToken;
        await _refreshTokenRepo.UpdateAsync(storedToken);

        return newAuthResponse;
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = _refreshTokenRepo.Table.FirstOrDefault(r => r.Token == refreshToken);

        if (storedToken != null && !storedToken.IsRevoked)
        {
            storedToken.IsRevoked = true;
            await _refreshTokenRepo.UpdateAsync(storedToken);
        }
    }
}
