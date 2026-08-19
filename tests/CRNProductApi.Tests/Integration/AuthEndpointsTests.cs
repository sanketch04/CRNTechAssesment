using System.Net;
using System.Net.Http.Json;
using Xunit;
using CRNProductApi.Application.DTOs.Auth;
using CRNProductApi.Application.DTOs.Common;

namespace CRNProductApi.Tests.Integration;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturn200_WhenValidRequest()
    {
        var registerDto = new RegisterDto
        {
            Email = "newuser@test.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data?.AccessToken);
        Assert.NotNull(apiResponse.Data?.RefreshToken);
    }

    [Fact]
    public async Task Login_ShouldReturn200_WhenValidCredentials()
    {
        var email = $"loginuser_{Guid.NewGuid()}@test.com";
        var registerDto = new RegisterDto { Email = email, Password = "Password123!" };

        await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        var loginDto = new LoginDto { Email = email, Password = "Password123!" };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data?.AccessToken);
    }

    [Fact]
    public async Task ProtectedEndpoint_ShouldReturn401_WhenNoTokenProvided()
    {
        var response = await _client.GetAsync("/api/v1/products");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
