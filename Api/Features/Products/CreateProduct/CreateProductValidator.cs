using FluentValidation;

namespace Api.Features.Products.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(150).WithMessage("El nombre no debe superar los 150 caracteres.");

        RuleFor(x => x.PriceAmount)
            .GreaterThan(0).WithMessage("El precio debe ser mayor que cero.");

        RuleFor(x => x.PriceCurrency)
            .NotEmpty().WithMessage("La moneda es requerida.")
            .Length(3).WithMessage("La moneda debe tener exactamente 3 caracteres (ej. USD).");
    }
}
