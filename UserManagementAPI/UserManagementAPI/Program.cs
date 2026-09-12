// Program.cs - application bootstrap and middleware pipeline setup.
//
// This file wires up Kestrel limits, services (EF Core, authentication, CORS,
// rate limiting, Swagger) and the middleware pipeline. Keep this file small
// and focused: business logic belongs in controllers/services, not here.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.RateLimiting;
using UserManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagementAPI.Middleware;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;


var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel limits: keep request bodies to a reasonable size to avoid
// excessive memory usage or DOS risk while still allowing valid uploads.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});

// Service Registration (controllers, EF Core, authentication, CORS, rate limiting)
// These registrations intentionally mirror the previous configuration. Any
// future service additions should be kept here and remain testable.
builder.Services.AddDbContext<UserManagementDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

// JWT configuration: pulled from configuration to keep secrets out of code.
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
builder.Services.AddAuthorization();

// CORS Policy: restrict in development to expected host. Update as needed for
// production deployment values.
builder.Services.AddCors(options =>
{
    options.AddPolicy("RestrictedCorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Rate Limiting: per-minute fixed window examples. Tune limits based on real
// traffic patterns rather than leaving these defaults in production.
builder.Services.AddRateLimiter(options =>
{
    // Global limiter
    options.AddFixedWindowLimiter("GlobalLimiter", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Authentication-related endpoints (e.g., login) can have stricter limits
    options.AddFixedWindowLimiter("AuthLimiter", limiter =>
    {
        limiter.PermitLimit = 5;                     // 5 requests
        limiter.Window = TimeSpan.FromMinutes(1);    // per minute
        limiter.QueueLimit = 0;                      // no queueing
    });
});

// Swagger/OpenAPI setup - keep for developer discovery and integration tests
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Development-only Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Input Sanitization Middleware
app.UseMiddleware<InputSanitizationMiddleware>();
app.UseMiddleware<SimpleExceptionMiddleware>();

// Input Sanitization Middleware
app.UseMiddleware<InputSanitizationMiddleware>();

// Error-handling middleware (must be first)
app.UseMiddleware<SimpleExceptionMiddleware>();

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});

app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'none'; img-src 'self' data:; script-src 'self'; " +
        "style-src 'self' 'unsafe-inline'; connect-src 'self'; " +
        "font-src 'self'; frame-ancestors 'none'; base-uri 'self';";
    await next();
});

// Rate Limiter
app.UseRateLimiter();

// CORS
app.UseCors("RestrictedCorsPolicy");

app.UseMiddleware<InputSanitizationMiddleware>();
app.UseMiddleware<SimpleExceptionMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Global Exception Handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var errorFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = errorFeature?.Error;

        var errorResponse = new
        {
            Message = "An unexpected error occurred.",
            Detail = exception?.Message,
            Path = errorFeature?.Path
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});

// Map Controllers
app.MapControllers();

app.Run();
