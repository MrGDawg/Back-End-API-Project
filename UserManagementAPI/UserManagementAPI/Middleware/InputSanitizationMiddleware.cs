using System.Text.RegularExpressions;

// Middleware that scans JSON request bodies for simple, common XSS-like patterns
// and rejects the request early with HTTP 400 to avoid processing potentially
// malicious input. This is intentionally lightweight and should not replace
// proper input validation or an HTML sanitizer for user-generated markup.
public class InputSanitizationMiddleware
{
    private readonly RequestDelegate _next;

    // Pattern targets common script injection tokens; keep this conservative
    // to avoid false positives but still provide basic protection during review.
    private static readonly Regex DangerousPattern = new Regex(
        @"(<script\b|javascript:|onerror=|onload=)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    public InputSanitizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // Inspect the JSON request body (if present). We enable buffering so the
    // downstream pipeline can still read the body. If a dangerous token is
    // detected, return 400 with a generic message to the client.
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType != null &&
            context.Request.ContentType.Contains("application/json",
                StringComparison.OrdinalIgnoreCase))
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            // Reset position so the controller can re-read the body
            context.Request.Body.Position = 0;

            if (!string.IsNullOrEmpty(body) && DangerousPattern.IsMatch(body))
            {
                // Return a minimal, non-sensitive error message to the client.
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "The request body contains potentially malicious content and was rejected."
                };

                await context.Response.WriteAsJsonAsync(response);
                return;
            }
        }

        await _next(context);
    }
}
