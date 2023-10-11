using MediatR;
using System.Diagnostics;

namespace SensoBackend.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) =>
        _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct
    )
    {
        var guid = Guid.NewGuid().ToString();
        _logger.LogInformation("Handling {} [{}]...", typeof(TRequest).Name, guid);
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();
        _logger.LogInformation(
            "Handled {} [{}] in {}ms!",
            typeof(TRequest).Name,
            guid,
            stopwatch.ElapsedMilliseconds
        );

        return response;
    }
}
