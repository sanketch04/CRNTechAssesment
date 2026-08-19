using System.Security.Claims;
using CRNProductApi.Application.DTOs.Auth;
using CRNProductApi.Domain.Entities;

namespace CRNProductApi.Application.Interfaces;

public interface ITokenService
{
    Task<AuthResponseDto> GenerateTokensAsync(ApplicationUser user, IEnumerable<string> roles);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    string GenerateRefreshToken();
}
