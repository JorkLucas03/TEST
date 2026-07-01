using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Api.Features;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;

        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        // Si la solicitud tarda más de 500ms, registrar una advertencia de rendimiento
        if (elapsedMilliseconds > 500)
        {
            _logger.LogWarning(
                "Solicitud lenta detectada: {RequestName} tardó {ElapsedMilliseconds} ms para ejecutarse.",
                requestName,
                elapsedMilliseconds
            );
        }

        return response;
    }
}
