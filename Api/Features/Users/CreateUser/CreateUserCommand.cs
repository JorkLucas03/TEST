using MediatR;

namespace Api.Features.Users.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email
) : IRequest<Guid>;
