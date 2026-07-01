using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;

namespace Api.Features.Products;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal PriceAmount,
    string PriceCurrency
) : IRequest<Guid>;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre es requerido.")
            .MaximumLength(150)
            .WithMessage("El nombre no debe superar los 150 caracteres.");

        RuleFor(x => x.PriceAmount)
            .GreaterThan(0)
            .WithMessage("El precio debe ser mayor que cero.");

        RuleFor(x => x.PriceCurrency)
            .NotEmpty()
            .WithMessage("La moneda es requerida.")
            .Length(3)
            .WithMessage("La moneda debe tener exactamente 3 caracteres (ej. USD).");
    }
}

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateProductHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var price = Money.Create(request.PriceAmount, request.PriceCurrency);
        var product = Product.Create(request.Name, request.Description, price);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Id.Value;
    }
}
