using CRNProductApi.Application.DTOs.Common;
using CRNProductApi.Application.DTOs.Product;

namespace CRNProductApi.Application.Interfaces;

public interface IProductService
{
    Task<PageResponse<ProductResponseDto>> GetAllProductsAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> GetProductByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> UpdateProductAsync(long id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(long id, CancellationToken cancellationToken = default);
}
