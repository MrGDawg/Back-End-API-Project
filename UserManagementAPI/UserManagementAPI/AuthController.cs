using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UserManagementAPI.Services;
using UserManagementAPI.Models;

/// <summary>
/// Controller responsible for authentication endpoints such as register and login.
/// The endpoints are rate-limited using the "AuthLimiter" policy to reduce
/// the risk of automated abuse during testing.
/// </summary>
[EnableRateLimiting("AuthLimiter")]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registration endpoint. Returns an <see cref="UserManagementAPI.Models.AuthResponse"/>
    /// indicating success/failure and a JWT when successful.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Login endpoint. Returns an <see cref="UserManagementAPI.Models.AuthResponse"/>
    /// containing a JWT when credentials are valid.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }
}
