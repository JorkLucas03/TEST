# TEST

Proyecto de tarea desarrollado con ASP.NET Core, .NET Aspire, Entity Framework Core y SQL Server.

La API sigue una organizacion inspirada en Clean Architecture: controladores en la capa de entrada, casos de uso en `Features`, entidades de dominio en `Domain` y acceso a datos en `Infrastructure`.

## Tecnologias

- .NET 10
- ASP.NET Core Web API
- .NET Aspire 13.4.6
- SQL Server ejecutado desde Aspire
- Entity Framework Core
- MediatR
- FluentValidation
- Vogen para value objects
- Bogus para datos de prueba
- CSharpier para formato de codigo

## Estructura

- `Api/`: proyecto principal de la Web API.
- `Api/Controllers/`: controladores HTTP.
- `Api/Domain/`: entidades, value objects y clases base del dominio.
- `Api/Features/`: casos de uso con comandos, consultas, handlers y validaciones.
- `Api/Infrastructure/`: configuracion de Entity Framework Core, `ApplicationDbContext` y seeding de datos.
- `Api.AppHost/`: AppHost de .NET Aspire para ejecutar la API junto con SQL Server.
- `Api/Api.http`: archivo para probar endpoints desde el editor.
- `Api.slnx`: solucion del proyecto.

## Funcionalidad Actual

- Listar usuarios.
- Crear usuarios.
- Listar productos.
- Crear productos.
- Inicializar la base de datos automaticamente si esta vacia.
- Sembrar datos de prueba de usuarios y productos con Bogus.
- Exponer OpenAPI en ambiente de desarrollo.

## Endpoints

La API esta configurada para responder en `http://localhost:5232` cuando se ejecuta desde Aspire.

### Usuarios

```http
GET /api/usuarios
GET /api/users
POST /api/usuarios
POST /api/users
```

Ejemplo de respuesta de `GET /api/usuarios`:

```json
[
  {
    "id": "3b145d94-309e-4a4f-aad3-f31e4e2966d4",
    "nombre": "Alphonso",
    "apellido": "Howe",
    "email": "0_Alphonso_Howe@gmail.com"
  }
]
```

Ejemplo para crear usuario:

```json
{
  "firstName": "Rosalia",
  "lastName": "Hernandez",
  "email": "rosalia.hernandez@example.com"
}
```

### Productos

```http
GET /api/productos
GET /api/products
POST /api/productos
POST /api/products
```

Ejemplo para crear producto:

```json
{
  "name": "Producto de prueba",
  "description": "Creado desde Api.http",
  "priceAmount": 25.50,
  "priceCurrency": "USD"
}
```

## Requisitos

- .NET SDK 10
- Docker, necesario para ejecutar SQL Server desde Aspire.
- Aspire CLI.

## Restaurar y Compilar

```bash
dotnet restore Api.slnx
dotnet build Api.slnx
```

## Ejecutar el Proyecto

La forma recomendada es ejecutar el AppHost con Aspire:

```bash
aspire run --apphost Api.AppHost/Api.AppHost.csproj
```

Cuando Aspire termine de iniciar, la API queda disponible en:

```text
http://localhost:5232
```

Tambien se puede abrir el dashboard de Aspire usando la URL que aparece en la terminal.

## Probar con Api.http

Abrir el archivo:

```text
Api/Api.http
```

Y ejecutar las peticiones con **Send Request**.

Peticiones incluidas:

- `GET /api/usuarios`
- `GET /api/productos`
- `POST /api/usuarios`
- `POST /api/productos`

## Base de Datos

El AppHost configura SQL Server con Aspire:

```csharp
var bd = builder
    .AddSqlServer("bdserver")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("bd");
```

La API recibe la conexion con:

```csharp
builder.AddSqlServerDbContext<ApplicationDbContext>("bd");
```

Al iniciar, se ejecuta `DbInitializer.SeedAsync(context)` para crear la base si no existe y agregar datos de prueba cuando las tablas estan vacias.

## Formato de Codigo

El proyecto usa CSharpier como herramienta local:

```bash
dotnet tool restore
dotnet csharpier format .
```

Para revisar formato sin modificar archivos:

```bash
dotnet csharpier check .
```

## OpenAPI

En desarrollo, la API expone el documento OpenAPI en:

```text
/openapi/v1.json
```
