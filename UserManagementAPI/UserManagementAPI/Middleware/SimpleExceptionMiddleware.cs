using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace UserManagementAPI.Middleware
{
    public class SimpleExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SimpleExceptionMiddleware> _logger;

        public SimpleExceptionMiddleware(RequestDelegate next, ILogger<SimpleExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Catch exceptions thrown by downstream middleware or controllers so we can
        // log diagnostics and return a stable, non-sensitive error response to the client.
        // Keeping the handler simple avoids leaking internal details while still
        // allowing the logger to capture stack traces for debugging.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log full details internally (safe for logs) to aid debugging.
                _logger.LogError(ex, "Unhandled exception occurred.");

                // Return safe, generic error to client (no stack trace)
                await HandleExceptionAsync(context);
            }
        }

        // Produces a small JSON problem response. This mirrors a subset of
        // RFC 7807-style Problem Details but keeps the payload intentionally
        // minimal for security and consistency during peer review.
        private static async Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new
            {
                status = context.Response.StatusCode,
                title = "An unexpected error occurred.",
                detail = "Please contact support if the issue persists."
            };

            var json = JsonSerializer.Serialize(problem);

            await context.Response.WriteAsync(json);
        }
    }
}
