using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.CreateUser;

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
        
        // Validar si el email ya existe para evitar errores de restricción de base de datos
        var emailExists = await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (emailExists)
        {
            throw new ArgumentException("El correo electrónico ya está registrado por otro usuario.");
        }

        var user = User.Create(request.FirstName, request.LastName, email);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return user.Id.Value;
    }
}
