using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRNProductApi.Application.DTOs.Common;
using CRNProductApi.Application.DTOs.Item;
using CRNProductApi.Application.Interfaces;
using CRNProductApi.Domain.Entities;

namespace CRNProductApi.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products/{productId:long}/items")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly IValidator<CreateItemDto> _createValidator;
    private readonly IValidator<UpdateItemDto> _updateValidator;

    public ItemsController(
        IItemService itemService,
        IValidator<CreateItemDto> createValidator,
        IValidator<UpdateItemDto> updateValidator)
    {
        _itemService = itemService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ItemResponseDto>>>> GetItems(long productId, CancellationToken cancellationToken)
    {
        var result = await _itemService.GetItemsByProductIdAsync(productId, cancellationToken);
        return Ok(ApiResponse<List<ItemResponseDto>>.Ok(result));
    }

    [HttpGet("{itemId:long}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ItemResponseDto>>> GetItem(long productId, long itemId, CancellationToken cancellationToken)
    {
        var result = await _itemService.GetItemByIdAsync(productId, itemId, cancellationToken);
        return Ok(ApiResponse<ItemResponseDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ItemResponseDto>>> CreateItem(long productId, [FromBody] CreateItemDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var result = await _itemService.CreateItemAsync(productId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetItem), new { productId, itemId = result.Id }, ApiResponse<ItemResponseDto>.Ok(result, "Item created successfully."));
    }

    [HttpPut("{itemId:long}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ItemResponseDto>>> UpdateItem(long productId, long itemId, [FromBody] UpdateItemDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var result = await _itemService.UpdateItemAsync(productId, itemId, dto, cancellationToken);
        return Ok(ApiResponse<ItemResponseDto>.Ok(result, "Item updated successfully."));
    }

    [HttpDelete("{itemId:long}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteItem(long productId, long itemId, CancellationToken cancellationToken)
    {
        await _itemService.DeleteItemAsync(productId, itemId, cancellationToken);
        return NoContent();
    }
}
