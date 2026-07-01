using Api.Domain.ValueObjects;
using Api.Features.Users;
using Api.Tests.Factories;
using Api.Tests.Setup;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Api.Tests.Features.Users.Commands;

public class CreateUserTests : IntegrationTestBase
{
    [Test]
    public async Task CreateUser_WithValidCommand_ShouldSaveUserToDatabase()
    {
        // Arrange
        var command = UserTestFactory.CreateRegisterCommand(firstName: "John", lastName: "Doe");

        // Act
        var userId = await SendAsync(command);

        // Assert
        userId.ShouldNotBe(Guid.Empty);

        await ExecuteDbContextAsync(async context =>
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Id == UserId.From(userId));
            user.ShouldNotBeNull();
            user.FirstName.ShouldBe("John");
            user.LastName.ShouldBe("Doe");
            user.Email.Value.ShouldBe(command.Email);
        });
    }

    [Test]
    public async Task CreateUser_WithDuplicateEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var email = "duplicate@example.com";
        var firstCommand = UserTestFactory.CreateRegisterCommand(email: email);
        var secondCommand = UserTestFactory.CreateRegisterCommand(email: email);

        await SendAsync(firstCommand);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await SendAsync(secondCommand));
    }

    [Test]
    public async Task CreateUser_WithInvalidData_ShouldThrowValidationException()
    {
        // Arrange
        var command = UserTestFactory.CreateRegisterCommand(firstName: "", email: "not-an-email");

        // Act & Assert
        var ex = await Should.ThrowAsync<ValidationException>(async () => await SendAsync(command));
        ex.Errors.ShouldNotBeEmpty();
    }

    // EDGE CASE: Nombre con longitud máxima exacta (100 caracteres) debe tener éxito
    [Test]
    public async Task CreateUser_WithFirstNameOfExactMaxLength_ShouldSucceed()
    {
        // Arrange
        var maxLengthName = new string('A', 100);
        var command = UserTestFactory.CreateRegisterCommand(firstName: maxLengthName);

        // Act
        var userId = await SendAsync(command);

        // Assert
        userId.ShouldNotBe(Guid.Empty);
        await ExecuteDbContextAsync(async context =>
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Id == UserId.From(userId));
            user.ShouldNotBeNull();
            user.FirstName.Length.ShouldBe(100);
        });
    }

    // EDGE CASE: Nombre que excede la longitud máxima permitida (101 caracteres) debe lanzar error de validación
    [Test]
    public async Task CreateUser_WithFirstNameExceedingMaxLength_ShouldThrowValidationException()
    {
        // Arrange
        var tooLongName = new string('A', 101);
        var command = UserTestFactory.CreateRegisterCommand(firstName: tooLongName);

        // Act & Assert
        var ex = await Should.ThrowAsync<ValidationException>(async () => await SendAsync(command));
        ex.Errors.Any(e => e.PropertyName == nameof(CreateUserCommand.FirstName)).ShouldBeTrue();
    }

    // EDGE CASE: Email con valores duplicados pero con mayúsculas/minúsculas diferentes debe rechazarse
    [Test]
    public async Task CreateUser_WithDuplicateEmailDifferentCasing_ShouldThrowException()
    {
        // Arrange
        var emailLower = "test.casing@example.com";
        var emailUpper = "TEST.CASING@EXAMPLE.COM";

        var firstCommand = UserTestFactory.CreateRegisterCommand(email: emailLower);
        var secondCommand = UserTestFactory.CreateRegisterCommand(email: emailUpper);

        await SendAsync(firstCommand);

        // Act & Assert
        await Should.ThrowAsync<Exception>(async () => await SendAsync(secondCommand));
    }
}
