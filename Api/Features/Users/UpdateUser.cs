using Api.Domain.ValueObjects;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users;

// 1. El Comando (DTO de Entrada)
public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email
) : IRequest;

// 2. El Validador (FluentValidation)
public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del usuario es requerido.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no debe superar los 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100).WithMessage("El apellido no debe superar los 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");
    }
}

// 3. El Manejador (Handler)
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateUserHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.From(request.Id);
        
        // Obtener el usuario activo
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"No se encontró ningún usuario con el identificador '{request.Id}'.");
        }

        var newEmail = Email.From(request.Email);

        // Validar si el email fue cambiado y si pertenece a otro usuario activo
        if (user.Email != newEmail)
        {
            var emailExists = await _dbContext.Users
                .AnyAsync(u => u.Id != userId && u.Email == newEmail, cancellationToken);

            if (emailExists)
            {
                throw new ArgumentException("El correo electrónico ya está registrado por otro usuario.");
            }
        }

        // Actualizar detalles en la entidad
        user.UpdateDetails(request.FirstName, request.LastName, newEmail);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
