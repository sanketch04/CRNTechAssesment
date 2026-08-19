using FluentValidation;
using CRNProductApi.Application.DTOs.Item;

namespace CRNProductApi.Application.Validators;

public class UpdateItemValidator : AbstractValidator<UpdateItemDto>
{
    public UpdateItemValidator()
    {
        RuleFor(i => i.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
