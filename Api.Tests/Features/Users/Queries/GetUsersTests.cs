using Api.Features.Users;
using Api.Tests.Factories;
using Api.Tests.Setup;
using FluentValidation;
using NUnit.Framework;
using Shouldly;

namespace Api.Tests.Features.Users.Queries;

public class GetUsersTests : IntegrationTestBase
{
    [Test]
    public async Task GetUsers_WithDefaultPagination_ShouldReturnPaginatedList()
    {
        // Arrange
        await ExecuteDbContextAsync(async context =>
        {
            for (int i = 1; i <= 12; i++)
            {
                var user = UserTestFactory.CreateUser();
                context.Users.Add(user);
            }
            await context.SaveChangesAsync();
        });

        // Act
        var query = new GetUsersQuery(PageNumber: 1, PageSize: 10);
        var result = await SendAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(10);
        result.TotalCount.ShouldBe(12);
        result.PageNumber.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.TotalPages.ShouldBe(2);
        result.HasNextPage.ShouldBeTrue();
        result.HasPreviousPage.ShouldBeFalse();
    }

    [Test]
    public async Task GetUsers_WithInvalidPaginationParameters_ShouldThrowValidationException()
    {
        // Arrange
        var query = new GetUsersQuery(PageNumber: 0, PageSize: 150);

        // Act & Assert
        var ex = await Should.ThrowAsync<ValidationException>(async () => await SendAsync(query));
        ex.Errors.Count().ShouldBe(2);
    }

    // EDGE CASE: Consultar en una base de datos vacía debe retornar una lista vacía con conteos en 0
    [Test]
    public async Task GetUsers_WhenDatabaseIsEmpty_ShouldReturnEmptyList()
    {
        // Act
        var query = new GetUsersQuery(PageNumber: 1, PageSize: 10);
        var result = await SendAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
        result.TotalPages.ShouldBe(0);
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeFalse();
    }

    // EDGE CASE: Solicitar un número de página que excede el total de páginas debe retornar lista vacía con conteo total correcto
    [Test]
    public async Task GetUsers_WhenPageNumberExceedsTotalPages_ShouldReturnEmptyListWithCorrectCount()
    {
        // Arrange
        await ExecuteDbContextAsync(async context =>
        {
            for (int i = 1; i <= 12; i++)
            {
                var user = UserTestFactory.CreateUser();
                context.Users.Add(user);
            }
            await context.SaveChangesAsync();
        });

        // Act
        var query = new GetUsersQuery(PageNumber: 3, PageSize: 10);
        var result = await SendAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(12);
        result.TotalPages.ShouldBe(2);
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeTrue(); // porque PageNumber (3) es mayor que 1
    }

    // EDGE CASE: Consultar con el tamaño de página máximo permitido (100) debe completarse correctamente
    [Test]
    public async Task GetUsers_WithMaxPageSize_ShouldSucceed()
    {
        // Act
        var query = new GetUsersQuery(PageNumber: 1, PageSize: 100);
        var result = await SendAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.PageSize.ShouldBe(100);
    }
}
