using FluentValidation;
using CRNProductApi.Application.DTOs.Product;

namespace CRNProductApi.Application.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(p => p.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Product name cannot be empty or whitespace.")
            .MaximumLength(255).WithMessage("Product name must not exceed 255 characters.");
    }
}
