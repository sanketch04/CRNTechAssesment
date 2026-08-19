using System.Linq.Expressions;
using Moq;
using Xunit;
using CRNProductApi.Application.DTOs.Common;
using CRNProductApi.Application.DTOs.Product;
using CRNProductApi.Application.Interfaces;
using CRNProductApi.Application.Services;
using CRNProductApi.Domain.Entities;
using CRNProductApi.Domain.Exceptions;
using CRNProductApi.Infrastructure.Repositories;

namespace CRNProductApi.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IRepository<Product>> _mockProductRepo;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepo = new Mock<IRepository<Product>>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockCurrentUserService.Setup(c => c.UserName).Returns("testuser@crn.com");

        _productService = new ProductService(_mockProductRepo.Object, _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldCreateProduct_WhenValidDto()
    {
        var createDto = new CreateProductDto { ProductName = "Test Product" };

        _mockProductRepo.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        var result = await _productService.CreateProductAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("Test Product", result.ProductName);
        Assert.Equal("testuser@crn.com", result.CreatedBy);
        _mockProductRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenExists()
    {
        var product = new Product { Id = 1, ProductName = "Existing Product", CreatedBy = "admin" };
        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _productService.GetProductByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Existing Product", result.ProductName);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldThrowNotFoundException_WhenDoesNotExist()
    {
        _mockProductRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetProductByIdAsync(99));
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnPagedProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, ProductName = "Product A", CreatedBy = "admin" },
            new Product { Id = 2, ProductName = "Product B", CreatedBy = "admin" }
        };

        var pageResponse = new PageResponse<Product>(products, 2, 1, 10);

        _mockProductRepo.Setup(r => r.GetAllAsync(1, 10, It.IsAny<Expression<Func<Product, object>>[]>()))
            .ReturnsAsync(pageResponse);

        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };

        var result = await _productService.GetAllProductsAsync(pageRequest);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldUpdateProduct_WhenExists()
    {
        var product = new Product { Id = 1, ProductName = "Old Name", CreatedBy = "admin" };
        var updateDto = new UpdateProductDto { ProductName = "New Name" };

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockProductRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        var result = await _productService.UpdateProductAsync(1, updateDto);

        Assert.Equal("New Name", result.ProductName);
        Assert.Equal("testuser@crn.com", result.ModifiedBy);
        _mockProductRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldDeleteProduct_WhenExists()
    {
        var product = new Product { Id = 1, ProductName = "To Delete", CreatedBy = "admin" };

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockProductRepo.Setup(r => r.DeleteAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        await _productService.DeleteProductAsync(1);

        _mockProductRepo.Verify(r => r.DeleteAsync(product), Times.Once);
    }
}
