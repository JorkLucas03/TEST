using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests.Setup;

public class ApiFactory : WebApplicationFactory<Api.Infrastructure.Data.ApplicationDbContext>
{
    private readonly string _connectionString;

    public ApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Configurar el entorno en "Testing" para omitir la inicialización automática en Program.cs
        builder.UseEnvironment("Testing");
        
        // builder.UseSetting aplica las configuraciones al IWebHostBuilder antes de que
        // se ejecute Program.cs, lo que soluciona problemas de inicialización de configuración
        // temprana con WebApplicationBuilder.
        builder.UseSetting("ConnectionStrings:bd", _connectionString);
    }
}
