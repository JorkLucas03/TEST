using Api.Domain.ValueObjects;
using Api.Features.Users;
using Api.Tests.Factories;
using Api.Tests.Setup;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Api.Tests.Features.Users.Commands;

public class DeleteUserTests : IntegrationTestBase
{
    [Test]
    public async Task DeleteUser_WithValidId_ShouldSoftDeleteUserAndExcludeFromQueries()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();
        Guid userId = Guid.Empty;

        await ExecuteDbContextAsync(async context =>
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            userId = user.Id.Value;
        });

        // Act
        var command = new DeleteUserCommand(userId);
        await SendAsync(command);

        // Assert
        // 1. Debe arrojar KeyNotFoundException si intentamos buscarlo (el filtro global lo oculta)
        await Should.ThrowAsync<KeyNotFoundException>(async () =>
        {
            await SendAsync(new GetUserByIdQuery(userId));
        });

        // 2. Comprobar que en la base de datos sigue existiendo físicamente pero con IsDeleted = true
        await ExecuteDbContextAsync(async context =>
        {
            // IgnoreQueryFilters() permite omitir temporalmente el filtro de soft delete
            var physicalUser = await context.Users
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(u => u.Id == UserId.From(userId));

            physicalUser.ShouldNotBeNull();
            physicalUser.IsDeleted.ShouldBeTrue();
        });
    }

    [Test]
    public async Task DeleteUser_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());

        // Act & Assert
        await Should.ThrowAsync<KeyNotFoundException>(async () => await SendAsync(command));
    }

    // EDGE CASE: El email de un usuario borrado lógicamente debe poder ser reutilizado por otro nuevo usuario activo
    [Test]
    public async Task DeleteUser_SoftDeletedEmail_ShouldBeReusableByNewUser()
    {
        // Arrange
        var email = "reusable@example.com";
        var userToDelete = UserTestFactory.CreateUser(email: email);
        Guid deletedUserId = Guid.Empty;

        await ExecuteDbContextAsync(async context =>
        {
            context.Users.Add(userToDelete);
            await context.SaveChangesAsync();
            deletedUserId = userToDelete.Id.Value;
        });

        // Borrar el primer usuario
        await SendAsync(new DeleteUserCommand(deletedUserId));

        // Act & Assert
        // Crear un nuevo comando con el mismo email para otro usuario
        var newRegisterCommand = UserTestFactory.CreateRegisterCommand(firstName: "New", lastName: "User", email: email);

        // Debe registrarse de forma exitosa sin violar el índice único de la base de datos (Unique Index Filter)
        var newUserId = await SendAsync(newRegisterCommand);
        
        newUserId.ShouldNotBe(Guid.Empty);
        newUserId.ShouldNotBe(deletedUserId);
    }
}
