namespace Api.Features.Products;

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal PriceAmount,
    string PriceCurrency,
    bool IsActive,
    DateTime CreatedAt
);
