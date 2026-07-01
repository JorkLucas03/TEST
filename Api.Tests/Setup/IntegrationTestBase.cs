using Api.Infrastructure.Data;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Api.Tests.Setup;

[TestFixture]
public abstract class IntegrationTestBase
{
    private static DistributedApplication? _appHost;
    private static ApiFactory? _factory;
    private static DbResetter? _resetter;
    private static string? _connectionString;

    [OneTimeSetUp]
    public static async Task GlobalSetup()
    {
        // 1. Iniciar .NET Aspire AppHost programáticamente
        var appHostTestingBuilder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Api_AppHost>();
        _appHost = await appHostTestingBuilder.BuildAsync();
        await _appHost.StartAsync();

        // 2. Obtener la cadena de conexión de la base de datos "bd" (SqlServer)
        _connectionString = await _appHost.GetConnectionStringAsync("bd");

        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("No se pudo obtener la cadena de conexión de la base de datos de Aspire.");
        }

        // 3. Iniciar la fábrica de la API apuntando a la base de datos de Aspire
        _factory = new ApiFactory(_connectionString);

        // 4. Asegurar la creación de las tablas de base de datos
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureDeletedAsync(); // Borrar base de datos previa para forzar actualización de esquema
        await context.Database.EnsureCreatedAsync();

        // Sembrar la base de datos de forma segura en ambiente de pruebas
        await DbInitializer.SeedAsync(context);

        // 5. Inicializar el resetter de Respawn
        _resetter = new DbResetter(_connectionString);
    }

    [SetUp]
    public async Task TestSetup()
    {
        // Reiniciar datos en cada prueba para mantener la independencia
        await _resetter!.ResetAsync();
    }

    [OneTimeTearDown]
    public static async Task GlobalTearDown()
    {
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }

        if (_appHost != null)
        {
            await _appHost.DisposeAsync();
        }
    }

    // Ejecuta comandos o consultas a través del pipeline de MediatR (ISender)
    protected async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _factory!.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
        return await mediator.Send(request);
    }

    // Sobrecarga para comandos de MediatR sin tipo de retorno (IRequest)
    protected async Task SendAsync(IRequest request)
    {
        using var scope = _factory!.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
        await mediator.Send(request);
    }

    // Permite ejecutar operaciones directamente contra el DbContext (para sembrar o verificar)
    protected async Task ExecuteDbContextAsync(Func<ApplicationDbContext, Task> action)
    {
        using var scope = _factory!.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await action(context);
    }

    protected async Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action)
    {
        using var scope = _factory!.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await action(context);
    }
}
