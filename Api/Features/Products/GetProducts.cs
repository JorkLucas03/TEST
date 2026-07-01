using Api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Products;

public record GetProductsQuery : IRequest<IReadOnlyList<ProductResponse>>;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductResponse>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetProductsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductResponse>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        var products = await _dbContext
            .Products.AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);

        return products
            .Select(product => new ProductResponse(
                product.Id.Value,
                product.Name,
                product.Description,
                product.Price.Amount,
                product.Price.Currency,
                product.IsActive,
                product.CreatedAt
            ))
            .ToList();
    }
}
