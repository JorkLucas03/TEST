using Api.Domain.ValueObjects;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users;

// 1. El Comando (DTO de Entrada)
public record DeleteUserCommand(Guid Id) : IRequest;

// 2. El Validador (FluentValidation)
public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del usuario es requerido.");
    }
}

// 3. El Manejador (Handler)
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteUserHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.From(request.Id);

        // Obtener el usuario activo
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"No se encontró ningún usuario con el identificador '{request.Id}'.");
        }

        // Ejecutar borrado lógico
        user.Delete();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
