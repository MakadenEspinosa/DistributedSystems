using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VideoGameExchange.Server.Models;
using VideoGameExchange.Server.Services;

namespace VideoGameExchange.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return Unauthorized(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Unauthorized",
                Status = 401,
                Detail = "Invalid credentials format"
            });
        }

        var response = await _authService.AuthenticateAsync(request);
        
        if (response == null)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            return Unauthorized(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Unauthorized",
                Status = 401,
                Detail = "Invalid email or password"
            });
        }

        _logger.LogInformation("User {UserId} logged in successfully", response.UserId);
        return Ok(response);
    }
}
