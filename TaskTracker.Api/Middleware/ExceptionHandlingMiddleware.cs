// -------------------------------------------------------
// ExceptionHandlingMiddleware.cs
// Catches unhandled exceptions and returns RFC 7807 ProblemDetails.
// -------------------------------------------------------
using System.Net;
using System.Text.Json;

namespace TaskTracker.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            // Log with context
            _logger.LogError(ex, "Unhandled exception for {Path}", ctx.Request.Path);

            ctx.Response.ContentType = "application/problem+json";
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new
            {
                type = "https://httpstatuses.com/500",
                title = "An unexpected error occurred.",
                status = 500,
                traceId = ctx.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(problem);
            await ctx.Response.WriteAsync(json);
        }
    }
}

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
