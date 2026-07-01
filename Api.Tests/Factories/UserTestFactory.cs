using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Features.Users;
using Bogus;

namespace Api.Tests.Factories;

public static class UserTestFactory
{
    private static readonly Faker Faker = new();

    public static User CreateUser(string? firstName = null, string? lastName = null, string? email = null)
    {
        var fName = firstName ?? Faker.Name.FirstName();
        var lName = lastName ?? Faker.Name.LastName();
        // Usar email exacto si es proporcionado, de lo contrario generar uno único
        var emailStr = email ?? (Faker.UniqueIndex + "_" + Faker.Internet.Email(fName, lName));
        
        return User.Create(fName, lName, Email.From(emailStr));
    }

    public static CreateUserCommand CreateRegisterCommand(string? firstName = null, string? lastName = null, string? email = null)
    {
        var fName = firstName ?? Faker.Name.FirstName();
        var lName = lastName ?? Faker.Name.LastName();
        // Usar email exacto si es proporcionado, de lo contrario generar uno único
        var emailStr = email ?? (Faker.UniqueIndex + "_" + Faker.Internet.Email(fName, lName));
        
        return new CreateUserCommand(fName, lName, emailStr);
    }
}
