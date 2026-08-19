using CRNProductApi.Application.DTOs.Item;

namespace CRNProductApi.Application.Interfaces;

public interface IItemService
{
    Task<List<ItemResponseDto>> GetItemsByProductIdAsync(long productId, CancellationToken cancellationToken = default);
    Task<ItemResponseDto> GetItemByIdAsync(long productId, long itemId, CancellationToken cancellationToken = default);
    Task<ItemResponseDto> CreateItemAsync(long productId, CreateItemDto dto, CancellationToken cancellationToken = default);
    Task<ItemResponseDto> UpdateItemAsync(long productId, long itemId, UpdateItemDto dto, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(long productId, long itemId, CancellationToken cancellationToken = default);
}
