using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using CRNProductApi.Application.DTOs.Auth;
using CRNProductApi.Application.DTOs.Common;
using CRNProductApi.Application.DTOs.Product;
using CRNProductApi.Domain.Entities;

namespace CRNProductApi.Tests.Integration;

public class ProductsEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var email = $"admin_{Guid.NewGuid()}@test.com";
        var registerDto = new RegisterDto
        {
            Email = email,
            Password = "Password123!",
            Role = UserRoles.Admin
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        return apiResponse!.Data!.AccessToken;
    }

    private async Task<string> GetUserTokenAsync()
    {
        var email = $"user_{Guid.NewGuid()}@test.com";
        var registerDto = new RegisterDto
        {
            Email = email,
            Password = "Password123!",
            Role = UserRoles.User
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        return apiResponse!.Data!.AccessToken;
    }

    [Fact]
    public async Task CreateProduct_ShouldReturn201_WhenAdminUser()
    {
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateProductDto { ProductName = "Integration Test Laptop" };
        var response = await _client.PostAsJsonAsync("/api/v1/products", createDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductResponseDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("Integration Test Laptop", apiResponse.Data?.ProductName);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturn403_WhenNonAdminUser()
    {
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateProductDto { ProductName = "Forbidden Laptop" };
        var response = await _client.PostAsJsonAsync("/api/v1/products", createDto);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturn400_WhenValidationFails()
    {
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateProductDto { ProductName = "" };
        var response = await _client.PostAsJsonAsync("/api/v1/products", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturn404_WhenProductDoesNotExist()
    {
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/v1/products/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
