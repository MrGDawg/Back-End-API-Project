using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        // Maximum body size to log (64 KB)
        private const int MaxLogBodySize = 64 * 1024;

        // Fields that must NEVER appear in logs
        private static readonly string[] SensitiveFields =
        {
            "password",
            "passwordHash",
            "passwordSalt",
            "token",
            "authorization",
            "email",
            "fullName",
            "department"
        };

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request
            var requestBody = await ReadRequestBodyAsync(context.Request);
            var sanitizedRequestBody = Sanitize(requestBody);

            _logger.LogInformation("Incoming Request: {Method} {Path} | Body: {Body}",
                context.Request.Method,
                context.Request.Path,
                sanitizedRequestBody);

            // Capture response
            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            // Log response
            var responseBody = await ReadResponseBodyAsync(context.Response);
            var sanitizedResponseBody = Sanitize(responseBody);

            _logger.LogInformation("Outgoing Response: {StatusCode} | Body: {Body}",
                context.Response.StatusCode,
                sanitizedResponseBody);

            // Write back to original stream
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            try
            {
                request.EnableBuffering();

                if (request.ContentLength == null || request.ContentLength == 0)
                    return string.Empty;

                if (request.ContentLength > MaxLogBodySize)
                    return "[Request body too large to log]";

                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;

                return body;
            }
            catch
            {
                return "[Unable to read request body]";
            }
        }

        private async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            try
            {
                response.Body.Seek(0, SeekOrigin.Begin);

                using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();

                response.Body.Seek(0, SeekOrigin.Begin);

                if (body.Length > MaxLogBodySize)
                    return "[Response body too large to log]";

                return body;
            }
            catch
            {
                return "[Unable to read response body]";
            }
        }

        private string Sanitize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return json;

            try
            {
                foreach (var field in SensitiveFields)
                {
                    json = Regex.Replace(
                        json,
                        $"\"{field}\"\\s*:\\s*\".*?\"",
                        $"\"{field}\":\"***\"",
                        RegexOptions.IgnoreCase);
                }

                return json;
            }
            catch
            {
                return "[Unable to sanitize body]";
            }
        }
    }
}
