using Api.Features;
using Api.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Registrar DBContext usando el componente .NET Aspire SQL Server
builder.AddSqlServerDbContext<ApplicationDbContext>("bd");

// Registrar dependencias de Features (MediatR, FluentValidation)
builder.Services.AddFeatures();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Inicializar y sembrar base de datos si está vacía
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

