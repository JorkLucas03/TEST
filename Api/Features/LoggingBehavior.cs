using MediatR;
using Microsoft.Extensions.Logging;

namespace Api.Features;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Iniciando ejecución de la solicitud: {RequestName}", requestName);

        try
        {
            var response = await next();
            _logger.LogInformation("Solicitud completada con éxito: {RequestName}", requestName);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico durante la ejecución de la solicitud: {RequestName}", requestName);
            throw;
        }
    }
}
