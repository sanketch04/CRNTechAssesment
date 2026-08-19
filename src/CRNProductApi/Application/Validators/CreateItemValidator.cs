using FluentValidation;
using CRNProductApi.Application.DTOs.Item;

namespace CRNProductApi.Application.Validators;

public class CreateItemValidator : AbstractValidator<CreateItemDto>
{
    public CreateItemValidator()
    {
        RuleFor(i => i.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
