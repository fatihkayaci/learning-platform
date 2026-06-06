using System.Text.Json;
using BuildingBlocks.Common.Idempotency;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Common.Behaviors;

public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand<TResponse>
{
    private const string IdempotencyKeyHeader = "Idempotency-Key";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

    public IdempotencyBehavior(IDistributedCache cache, IHttpContextAccessor httpContextAccessor, ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null || !httpContext.Request.Headers.TryGetValue(IdempotencyKeyHeader, out Microsoft.Extensions.Primitives.StringValues keyValues))
            return await next(cancellationToken);

        string idempotencyKey = keyValues.ToString();
        string userId = httpContext.User.FindFirst("sub")?.Value ?? "anonymous";
        string cacheKey = $"idempotency:{typeof(TRequest).Name}:{userId}:{idempotencyKey}";

        string? cachedResponse = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            _logger.LogInformation("Idempotent request detected for {CommandName} with key {IdempotencyKey}", typeof(TRequest).Name, idempotencyKey);
            return JsonSerializer.Deserialize<TResponse>(cachedResponse)!;
        }

        TResponse response = await next(cancellationToken);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl },
            cancellationToken);

        return response;
    }
}
