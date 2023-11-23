using System.Diagnostics;
using MediatR;

namespace SensoBackend.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct
    )
    {
        var guid = Guid.NewGuid().ToString();
        logger.LogInformation("Handling {} [{}]...", typeof(TRequest).Name, guid);
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();
        logger.LogInformation(
            "Handled {} [{}] in {}ms!",
            typeof(TRequest).Name,
            guid,
            stopwatch.ElapsedMilliseconds
        );

        return response;
    }
}
