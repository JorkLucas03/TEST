using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // 1. Asegurar la creación de la base de datos (EnsureCreated creará el esquema si no existe)
        await context.Database.EnsureCreatedAsync();

        // 2. Sembrar Productos si no existen
        if (!await context.Products.AnyAsync())
        {
            var faker = new Faker();
            var products = new List<Product>();

            for (int i = 0; i < 20; i++)
            {
                var price = Money.Create(faker.Random.Decimal(10, 500), "USD");
                var product = Product.Create(
                    faker.Commerce.ProductName(),
                    faker.Commerce.ProductDescription(),
                    price
                );
                products.Add(product);
            }

            context.Products.AddRange(products);
        }

        // 3. Sembrar Usuarios si no existen
        if (!await context.Users.AnyAsync())
        {
            var faker = new Faker();
            var users = new List<User>();

            for (int i = 0; i < 15; i++)
            {
                var firstName = faker.Name.FirstName();
                var lastName = faker.Name.LastName();
                // Generar un email único y válido para Vogen usando faker.UniqueIndex
                var emailStr = faker.UniqueIndex + "_" + faker.Internet.Email(firstName, lastName);
                var email = Email.From(emailStr);

                var user = User.Create(firstName, lastName, email);
                users.Add(user);
            }

            context.Users.AddRange(users);
        }

        // 4. Guardar cambios si hubo inserciones
        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
        }
    }
}
