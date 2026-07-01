using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users;

// 1. El DTO de salida paginado
public record PagedResult<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

// 2. La Consulta (Query) con valores por defecto
public record GetUsersQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PagedResult<UserResponse>>;

// DTO individual de usuario
public record UserResponse(Guid Id, string Nombre, string Apellido, string Email);

// 3. El Validador (FluentValidation)
public class GetUsersValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El número de página debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El tamaño de página debe ser mayor o igual a 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("El tamaño de página no puede exceder los 100 registros.");
    }
}

// 4. El Manejador (Handler)
public class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResult<UserResponse>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetUsersHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<UserResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _dbContext.Users.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(user => new UserResponse(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Email.Value
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserResponse>(
            users,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
