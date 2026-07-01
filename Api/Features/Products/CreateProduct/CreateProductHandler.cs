using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Data;
using MediatR;

namespace Api.Features.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateProductHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var price = Money.Create(request.PriceAmount, request.PriceCurrency);
        var product = Product.Create(request.Name, request.Description, price);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Id.Value;
    }
}
