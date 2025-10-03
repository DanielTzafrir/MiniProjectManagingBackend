using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MiniProjectManager.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(httpContext, ex, StatusCodes.Status401Unauthorized);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(httpContext, ex, StatusCodes.Status404NotFound);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, StatusCodes.Status500InternalServerError);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode)
    {
        _logger.LogError(exception, "Error occurred");
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        object errorResponse;
        if (_env.IsDevelopment())
        {
            errorResponse = new { Message = exception.Message, InnerMessage = exception.InnerException?.Message, StackTrace = exception.StackTrace };
        }
        else
        {
            errorResponse = new { Message = exception.Message };
        }
        var json = JsonSerializer.Serialize(errorResponse);
        return context.Response.WriteAsync(json);
    }
}