using Moq;
using Xunit;
using CRNProductApi.Application.DTOs.Item;
using CRNProductApi.Application.Services;
using CRNProductApi.Domain.Entities;
using CRNProductApi.Domain.Exceptions;
using CRNProductApi.Infrastructure.Repositories;

namespace CRNProductApi.Tests.Services;

public class ItemServiceTests
{
    private readonly Mock<IRepository<Product>> _mockProductRepo;
    private readonly Mock<IRepository<Item>> _mockItemRepo;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _mockProductRepo = new Mock<IRepository<Product>>();
        _mockItemRepo = new Mock<IRepository<Item>>();

        _itemService = new ItemService(_mockItemRepo.Object, _mockProductRepo.Object);
    }

    [Fact]
    public async Task CreateItemAsync_ShouldCreateItem_WhenProductExists()
    {
        var product = new Product { Id = 1, ProductName = "Sample Product" };
        var createDto = new CreateItemDto { Quantity = 5 };

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockItemRepo.Setup(r => r.AddAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);

        var result = await _itemService.CreateItemAsync(1, createDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.ProductId);
        Assert.Equal(5, result.Quantity);
        _mockItemRepo.Verify(r => r.AddAsync(It.IsAny<Item>()), Times.Once);
    }

    [Fact]
    public async Task CreateItemAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        _mockProductRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var createDto = new CreateItemDto { Quantity = 5 };

        await Assert.ThrowsAsync<NotFoundException>(() => _itemService.CreateItemAsync(99, createDto));
    }

    [Fact]
    public async Task GetItemByIdAsync_ShouldReturnItem_WhenExists()
    {
        var product = new Product { Id = 1, ProductName = "Sample Product" };
        var item = new Item { Id = 10, ProductId = 1, Quantity = 3 };

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockItemRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(item);

        var result = await _itemService.GetItemByIdAsync(1, 10);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal(1, result.ProductId);
        Assert.Equal(3, result.Quantity);
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldUpdateItem_WhenValid()
    {
        var product = new Product { Id = 1, ProductName = "Sample Product" };
        var item = new Item { Id = 10, ProductId = 1, Quantity = 3 };
        var updateDto = new UpdateItemDto { Quantity = 20 };

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockItemRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(item);
        _mockItemRepo.Setup(r => r.UpdateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);

        var result = await _itemService.UpdateItemAsync(1, 10, updateDto);

        Assert.Equal(20, result.Quantity);
        _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldDeleteItem_WhenValid()
    {
        var product = new Product { Id = 1, ProductName = "Sample Product" };
        var item = new Item { Id = 10, ProductId = 1, Quantity = 3 };

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockItemRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(item);
        _mockItemRepo.Setup(r => r.DeleteAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);

        await _itemService.DeleteItemAsync(1, 10);

        _mockItemRepo.Verify(r => r.DeleteAsync(item), Times.Once);
    }
}
