using System.Diagnostics;
using ECommerceBackend.Logging;
using ECommerceBackend.Models;

namespace ECommerceBackend.Middleware;

public class CustomRequestLoggingMiddleware
{
private readonly RequestDelegate _next;

public CustomRequestLoggingMiddleware(
    RequestDelegate next)
{
    _next = next;
}

public async Task InvokeAsync(
    HttpContext context,
    IApplicationLogger logger)
{
    var requestStartTime =
        DateTime.UtcNow;

    var stopwatch =
        Stopwatch.StartNew();

    var correlationId =
        context.Request.Headers["X-Correlation-ID"]
            .FirstOrDefault();

    if (string.IsNullOrWhiteSpace(correlationId))
    {
        correlationId =
            Guid.NewGuid().ToString();
    }

    context.Response.Headers["X-Correlation-ID"] =
        correlationId;

    Exception? requestException = null;

    try
    {
        await _next(context);
    }
    catch (Exception ex)
    {
        requestException = ex;

        throw;
    }
    finally
    {
        stopwatch.Stop();

        var queryString =
            context.Request.QueryString.HasValue
                ? context.Request.QueryString.Value!
                : string.Empty;

        var clientIp =
            context.Connection.RemoteIpAddress?.ToString()
            ?? "Unknown";

        var statusCode =
            context.Response.StatusCode;

        // If an unhandled exception occurred,
        // treat the request as an error.
        var level =
            requestException != null ||
            statusCode >= 500
                ? "Error"
                : statusCode >= 400
                    ? "Warning"
                    : "Information";

        var message =
            requestException != null
                ? $"HTTP request failed: {requestException.Message}"
                : "HTTP request processed";

        var log =
            new ApplicationLog
            {
                Level =
                    level,

                HttpMethod =
                    context.Request.Method,

                RequestPath =
                    context.Request.Path,

                QueryString =
                    queryString,

                RequestStartTime =
                    requestStartTime,

                ResponseStatusCode =
                    statusCode,

                ExecutionDurationMs =
                    stopwatch.ElapsedMilliseconds,

                ClientIp =
                    clientIp,

                CorrelationId =
                    correlationId,

                Message =
                    message
            };

        try
        {
            await logger.LogAsync(log);
        }
        catch
        {
            // Do not allow a logging failure
            // to break the HTTP request.
        }
    }
}

}