using System.Linq.Expressions;
using CRNProductApi.Application.DTOs.Common;
using CRNProductApi.Application.DTOs.Item;
using CRNProductApi.Application.DTOs.Product;
using CRNProductApi.Application.Interfaces;
using CRNProductApi.Domain.Entities;
using CRNProductApi.Domain.Exceptions;
using CRNProductApi.Infrastructure.Repositories;

namespace CRNProductApi.Application.Services;

//Service
public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepo;
    private readonly ICurrentUserService _currentUserService;

    public ProductService(IRepository<Product> productRepo, ICurrentUserService currentUserService)
    {
        _productRepo = productRepo;
        _currentUserService = currentUserService;
    }

    public async Task<PageResponse<ProductResponseDto>> GetAllProductsAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var includes = new Expression<Func<Product, object>>[]
        {
            p => p.Items
        };

        if (!string.IsNullOrWhiteSpace(pageRequest.Search))
        {
            var search = pageRequest.Search.Trim().ToLower();
            var query = _productRepo.Table
                .Where(p => p.ProductName.ToLower().Contains(search));

            var totalCount = query.Count();
            var items = query
                .OrderBy(p => p.Id)
                .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(p => MapToDto(p))
                .ToList();

            return new PageResponse<ProductResponseDto>(items, totalCount, pageRequest.PageNumber, pageRequest.PageSize);
        }

        var pagedEntity = await _productRepo.GetAllAsync(pageRequest.PageNumber, pageRequest.PageSize, includes);
        var dtos = pagedEntity.Items.Select(p => MapToDto(p)).ToList();

        return new PageResponse<ProductResponseDto>(dtos, pagedEntity.TotalCount, pagedEntity.PageNumber, pagedEntity.PageSize);
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException(nameof(Product), id);
        }

        return MapToDto(product);
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var user = _currentUserService.UserName ?? "System";

        var product = new Product
        {
            ProductName = dto.ProductName.Trim(),
            CreatedBy = user,
            CreatedOn = DateTime.UtcNow
        };

        await _productRepo.AddAsync(product);
        return MapToDto(product);
    }

    public async Task<ProductResponseDto> UpdateProductAsync(long id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException(nameof(Product), id);
        }

        var user = _currentUserService.UserName ?? "System";

        product.ProductName = dto.ProductName.Trim();
        product.ModifiedBy = user;
        product.ModifiedOn = DateTime.UtcNow;

        await _productRepo.UpdateAsync(product);
        return MapToDto(product);
    }

    public async Task DeleteProductAsync(long id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException(nameof(Product), id);
        }

        await _productRepo.DeleteAsync(product);
    }

    private static ProductResponseDto MapToDto(Product p)
    {
        return new ProductResponseDto
        {
            Id = p.Id,
            ProductName = p.ProductName,
            CreatedBy = p.CreatedBy,
            CreatedOn = p.CreatedOn,
            ModifiedBy = p.ModifiedBy,
            ModifiedOn = p.ModifiedOn,
            Items = p.Items != null ? p.Items.Select(i => new ItemResponseDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList() : new List<ItemResponseDto>()
        };
    }
}
