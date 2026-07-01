using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users;

public record CreateUserCommand(string FirstName, string LastName, string Email) : IRequest<Guid>;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("El nombre es requerido.")
            .MaximumLength(100)
            .WithMessage("El nombre no debe superar los 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("El apellido es requerido.")
            .MaximumLength(100)
            .WithMessage("El apellido no debe superar los 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El correo electrónico es requerido.")
            .EmailAddress()
            .WithMessage("El formato del correo electrónico no es válido.");
    }
}

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateUserHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var email = Email.From(request.Email);

        var emailExists = await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (emailExists)
        {
            throw new ArgumentException(
                "El correo electrónico ya está registrado por otro usuario."
            );
        }

        var user = User.Create(request.FirstName, request.LastName, email);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return user.Id.Value;
    }
}
