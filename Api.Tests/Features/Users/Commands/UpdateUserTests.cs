using Api.Domain.ValueObjects;
using Api.Features.Users;
using Api.Tests.Factories;
using Api.Tests.Setup;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Api.Tests.Features.Users.Commands;

public class UpdateUserTests : IntegrationTestBase
{
    [Test]
    public async Task UpdateUser_WithValidCommand_ShouldUpdateUserInDatabase()
    {
        // Arrange
        var user = UserTestFactory.CreateUser(firstName: "Original", lastName: "Name");
        Guid userId = Guid.Empty;

        await ExecuteDbContextAsync(async context =>
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            userId = user.Id.Value;
        });

        var command = new UpdateUserCommand(userId, "Updated", "Name", "updated.email@example.com");

        // Act
        await SendAsync(command);

        // Assert
        await ExecuteDbContextAsync(async context =>
        {
            var updatedUser = await context.Users.SingleOrDefaultAsync(u => u.Id == UserId.From(userId));
            updatedUser.ShouldNotBeNull();
            updatedUser.FirstName.ShouldBe("Updated");
            updatedUser.LastName.ShouldBe("Name");
            updatedUser.Email.Value.ShouldBe("updated.email@example.com");
        });
    }

    [Test]
    public async Task UpdateUser_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "Jane", "Doe", "jane.doe@example.com");

        // Act & Assert
        await Should.ThrowAsync<KeyNotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task UpdateUser_WithDuplicateEmailOfOtherUser_ShouldThrowArgumentException()
    {
        // Arrange
        var email = "taken@example.com";
        var user1 = UserTestFactory.CreateUser(email: email);
        var user2 = UserTestFactory.CreateUser();
        Guid user2Id = Guid.Empty;

        await ExecuteDbContextAsync(async context =>
        {
            context.Users.Add(user1);
            context.Users.Add(user2);
            await context.SaveChangesAsync();
            user2Id = user2.Id.Value;
        });

        var command = new UpdateUserCommand(user2Id, "Jane", "Doe", email);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task UpdateUser_WithInvalidData_ShouldThrowValidationException()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.Empty, "", "Doe", "invalid-email");

        // Act & Assert
        var ex = await Should.ThrowAsync<ValidationException>(async () => await SendAsync(command));
        ex.Errors.Count().ShouldBe(3);
    }
}
