using FluentValidation;
using Labs_6.DTOs;

namespace Labs_6.Validators;

public class ProductWarehouseRequestValidator : AbstractValidator<CreateProductWarehouseRequest>
{
    public ProductWarehouseRequestValidator()
    {
        RuleFor(e => e.IdProduct).NotNull();
        RuleFor(e => e.IdWarehouse).NotNull();
        RuleFor(e => e.Amount).NotNull().GreaterThan(0);
        RuleFor(e => e.CreatedAt).NotNull();
    }
}