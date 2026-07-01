using MediatR;

namespace Api.Features.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal PriceAmount,
    string PriceCurrency
) : IRequest<Guid>;
