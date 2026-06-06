using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Common.Extensions;

public static class RedisExtensions
{
    public static WebApplicationBuilder AddRedisCache(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration["Redis:ConnectionString"]!;

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
        });

        return builder;
    }
}
