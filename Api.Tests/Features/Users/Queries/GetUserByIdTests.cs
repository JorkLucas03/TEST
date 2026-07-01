using Api.Features.Users;
using Api.Tests.Factories;
using Api.Tests.Setup;
using FluentValidation;
using NUnit.Framework;
using Shouldly;

namespace Api.Tests.Features.Users.Queries;

public class GetUserByIdTests : IntegrationTestBase
{
    [Test]
    public async Task GetUserById_WithValidId_ShouldReturnUser()
    {
        // Arrange
        // Sembrar un usuario e interceptar su ID en base de datos
        Guid seededUserId = Guid.Empty;
        var seededUser = UserTestFactory.CreateUser();

        await ExecuteDbContextAsync(async context =>
        {
            context.Users.Add(seededUser);
            await context.SaveChangesAsync();
            seededUserId = seededUser.Id.Value;
        });

        // Act
        var query = new GetUserByIdQuery(seededUserId);
        var result = await SendAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(seededUserId);
        result.Nombre.ShouldBe(seededUser.FirstName);
        result.Apellido.ShouldBe(seededUser.LastName);
        result.Email.ShouldBe(seededUser.Email.Value);
    }

    // EDGE CASE: Buscar un ID aleatorio que no existe debe lanzar KeyNotFoundException
    [Test]
    public async Task GetUserById_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var randomId = Guid.NewGuid();
        var query = new GetUserByIdQuery(randomId);

        // Act & Assert
        await Should.ThrowAsync<KeyNotFoundException>(async () => await SendAsync(query));
    }

    // EDGE CASE: Buscar con un identificador vacío (Guid.Empty) debe lanzar ValidationException
    [Test]
    public async Task GetUserById_WithEmptyGuidId_ShouldThrowValidationException()
    {
        // Arrange
        var query = new GetUserByIdQuery(Guid.Empty);

        // Act & Assert
        var ex = await Should.ThrowAsync<ValidationException>(async () => await SendAsync(query));
        ex.Errors.ShouldNotBeEmpty();
        ex.Errors.Any(e => e.PropertyName == nameof(GetUserByIdQuery.Id)).ShouldBeTrue();
    }
}
