using CRNProductApi.Application.DTOs.Item;
using CRNProductApi.Application.Interfaces;
using CRNProductApi.Domain.Entities;
using CRNProductApi.Domain.Exceptions;
using CRNProductApi.Infrastructure.Repositories;

namespace CRNProductApi.Application.Services;

//Service
public class ItemService : IItemService
{
    private readonly IRepository<Item> _itemRepo;
    private readonly IRepository<Product> _productRepo;

    public ItemService(IRepository<Item> itemRepo, IRepository<Product> productRepo)
    {
        _itemRepo = itemRepo;
        _productRepo = productRepo;
    }

    public async Task<List<ItemResponseDto>> GetItemsByProductIdAsync(long productId, CancellationToken cancellationToken = default)
    {
        await EnsureProductExistsAsync(productId);

        var items = _itemRepo.Table
            .Where(i => i.ProductId == productId)
            .Select(i => MapToDto(i))
            .ToList();

        return items;
    }

    public async Task<ItemResponseDto> GetItemByIdAsync(long productId, long itemId, CancellationToken cancellationToken = default)
    {
        await EnsureProductExistsAsync(productId);

        var item = await _itemRepo.GetByIdAsync(itemId);
        if (item == null || item.ProductId != productId)
        {
            throw new NotFoundException(nameof(Item), itemId);
        }

        return MapToDto(item);
    }

    public async Task<ItemResponseDto> CreateItemAsync(long productId, CreateItemDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureProductExistsAsync(productId);

        var item = new Item
        {
            ProductId = productId,
            Quantity = dto.Quantity
        };

        await _itemRepo.AddAsync(item);
        return MapToDto(item);
    }

    public async Task<ItemResponseDto> UpdateItemAsync(long productId, long itemId, UpdateItemDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureProductExistsAsync(productId);

        var item = await _itemRepo.GetByIdAsync(itemId);
        if (item == null || item.ProductId != productId)
        {
            throw new NotFoundException(nameof(Item), itemId);
        }

        item.Quantity = dto.Quantity;

        await _itemRepo.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task DeleteItemAsync(long productId, long itemId, CancellationToken cancellationToken = default)
    {
        await EnsureProductExistsAsync(productId);

        var item = await _itemRepo.GetByIdAsync(itemId);
        if (item == null || item.ProductId != productId)
        {
            throw new NotFoundException(nameof(Item), itemId);
        }

        await _itemRepo.DeleteAsync(item);
    }

    private async Task EnsureProductExistsAsync(long productId)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
        {
            throw new NotFoundException(nameof(Product), productId);
        }
    }

    private static ItemResponseDto MapToDto(Item item)
    {
        return new ItemResponseDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity
        };
    }
}
