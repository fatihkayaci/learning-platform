using Enrollment.Domain.Exceptions;
using System.Text.Json;

namespace Enrollment.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business rule violated: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (FluentValidation.ValidationException ex)
        {
            List<string> errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Validation failed: {Errors}", string.Join("; ", errors));

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { statusCode = 400, message = "Validation failed", errors });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await WriteResponse(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { statusCode, message }));
    }
}
