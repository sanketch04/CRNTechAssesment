using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using CRNProductApi.Application.DTOs.Auth;
using CRNProductApi.Application.DTOs.Common;
using CRNProductApi.Application.DTOs.Item;
using CRNProductApi.Application.DTOs.Product;
using CRNProductApi.Domain.Entities;

namespace CRNProductApi.Tests.Integration;

public class ItemsEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ItemsEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var email = $"admin_item_{Guid.NewGuid()}@test.com";
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

    [Fact]
    public async Task CreateItem_ShouldReturn201_WhenAdminAndProductExists()
    {
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productDto = new CreateProductDto { ProductName = "Product For Item Test" };
        var productResponse = await _client.PostAsJsonAsync("/api/v1/products", productDto);
        var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductResponseDto>>();
        var productId = productResult!.Data!.Id;

        var createItemDto = new CreateItemDto { Quantity = 10 };
        var itemResponse = await _client.PostAsJsonAsync($"/api/v1/products/{productId}/items", createItemDto);

        Assert.Equal(HttpStatusCode.Created, itemResponse.StatusCode);

        var itemResult = await itemResponse.Content.ReadFromJsonAsync<ApiResponse<ItemResponseDto>>();
        Assert.NotNull(itemResult);
        Assert.True(itemResult.Success);
        Assert.Equal(10, itemResult.Data?.Quantity);
        Assert.Equal(productId, itemResult.Data?.ProductId);
    }
}
