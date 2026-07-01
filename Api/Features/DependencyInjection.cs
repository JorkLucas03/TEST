using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Features;

public static class DependencyInjection
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Registrar MediatR
        services.AddMediatR(configuration => 
        {
            configuration.RegisterServicesFromAssembly(assembly);
        });

        // Registrar todos los validadores de FluentValidation en el assembly
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
