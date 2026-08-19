using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using CRNProductApi.Application.DTOs.Auth;
using CRNProductApi.Application.Interfaces;
using CRNProductApi.Application.Services;
using CRNProductApi.Domain.Entities;
using CRNProductApi.Infrastructure.Repositories;

namespace CRNProductApi.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<RoleManager<IdentityRole<long>>> _mockRoleManager;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IRepository<RefreshToken>> _mockRefreshTokenRepo;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var roleStore = new Mock<IRoleStore<IdentityRole<long>>>();
        _mockRoleManager = new Mock<RoleManager<IdentityRole<long>>>(roleStore.Object, null!, null!, null!, null!);

        _mockTokenService = new Mock<ITokenService>();
        _mockRefreshTokenRepo = new Mock<IRepository<RefreshToken>>();

        _authService = new AuthService(
            _mockUserManager.Object,
            _mockRoleManager.Object,
            _mockTokenService.Object,
            _mockRefreshTokenRepo.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
    {
        _mockUserManager.Setup(u => u.FindByEmailAsync("nonexistent@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var loginDto = new LoginDto { Email = "nonexistent@test.com", Password = "Password123" };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTokens_WhenCredentialsAreValid()
    {
        var user = new ApplicationUser { Id = 1, Email = "user@test.com", UserName = "user@test.com" };
        var authResponse = new AuthResponseDto
        {
            AccessToken = "valid_access_token",
            RefreshToken = "valid_refresh_token",
            Email = "user@test.com",
            Role = "User"
        };

        _mockUserManager.Setup(u => u.FindByEmailAsync("user@test.com")).ReturnsAsync(user);
        _mockUserManager.Setup(u => u.CheckPasswordAsync(user, "Password123")).ReturnsAsync(true);
        _mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

        _mockTokenService.Setup(t => t.GenerateTokensAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(authResponse);

        var loginDto = new LoginDto { Email = "user@test.com", Password = "Password123" };

        var result = await _authService.LoginAsync(loginDto);

        Assert.NotNull(result);
        Assert.Equal("valid_access_token", result.AccessToken);
        Assert.Equal("valid_refresh_token", result.RefreshToken);
    }
}
