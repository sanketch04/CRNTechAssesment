using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRNProductApi.Application.DTOs.Common;
using CRNProductApi.Application.DTOs.Product;
using CRNProductApi.Application.Interfaces;
using CRNProductApi.Domain.Entities;

namespace CRNProductApi.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductsController(
        IProductService productService,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PageResponse<ProductResponseDto>>>> GetAll([FromQuery] PageRequest pageRequest, CancellationToken cancellationToken)
    {
        var result = await _productService.GetAllProductsAsync(pageRequest, cancellationToken);
        return Ok(ApiResponse<PageResponse<ProductResponseDto>>.Ok(result));
    }

    [HttpGet("{id:long}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ProductResponseDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var result = await _productService.CreateProductAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ProductResponseDto>.Ok(result, "Product created successfully."));
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(long id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var result = await _productService.UpdateProductAsync(id, dto, cancellationToken);
        return Ok(ApiResponse<ProductResponseDto>.Ok(result, "Product updated successfully."));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await _productService.DeleteProductAsync(id, cancellationToken);
        return NoContent();
    }
}
