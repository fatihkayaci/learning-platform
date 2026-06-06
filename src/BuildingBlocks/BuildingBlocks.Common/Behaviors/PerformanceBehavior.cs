using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int SlowRequestThresholdMs = 500;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        TResponse response = await next(cancellationToken);

        stopwatch.Stop();
        long elapsed = stopwatch.ElapsedMilliseconds;
        string handlerName = typeof(TRequest).Name;

        if (elapsed >= SlowRequestThresholdMs)
            _logger.LogWarning("Slow request detected: {HandlerName} took {Elapsed}ms", handlerName, elapsed);
        else
            _logger.LogInformation("Request {HandlerName} completed in {Elapsed}ms", handlerName, elapsed);

        return response;
    }
}
