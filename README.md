# TEST

Proyecto de tarea desarrollado con ASP.NET Core y .NET Aspire.

## Estructura

- `Api/`: Web API de ASP.NET Core.
- `Api.AppHost/`: AppHost de .NET Aspire para ejecutar la API junto con SQL Server.
- `Api.slnx`: solucion del proyecto.

## Requisitos

- .NET SDK 10
- Docker, necesario para ejecutar SQL Server desde Aspire.
- Aspire CLI, si se quiere ejecutar desde la terminal.

## Ejecucion

Restaurar y compilar:

```bash
dotnet restore Api.slnx
dotnet build Api.slnx
```

Ejecutar el AppHost:

```bash
dotnet run --project Api.AppHost/Api.AppHost.csproj
```

En desarrollo, la API expone OpenAPI en `/openapi/v1.json`.
