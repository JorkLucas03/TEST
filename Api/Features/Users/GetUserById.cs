using Api.Domain.ValueObjects;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users;

// 1. La Consulta (Query)
public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse>;

// 2. El Validador (FluentValidation)
public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador de usuario es requerido.");
    }
}

// 3. El Manejador (Handler)
public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetUserByIdHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = UserId.From(request.Id);
        
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"No se encontró ningún usuario con el identificador '{request.Id}'.");
        }

        return new UserResponse(
            user.Id.Value,
            user.FirstName,
            user.LastName,
            user.Email.Value
        );
    }
}
