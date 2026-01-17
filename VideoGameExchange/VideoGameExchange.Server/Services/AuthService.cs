using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VideoGameExchange.Server.Configuration;
using VideoGameExchange.Server.Models;

namespace VideoGameExchange.Server.Services;

public class AuthService
{
    private readonly UserService _userService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(UserService userService, IOptions<JwtSettings> jwtSettings)
    {
        _userService = userService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _userService.GetUserByEmailAsync(request.Email);
        if (user == null)
            return null;

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return null;

        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);

        return new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            ExpiresAt = expiresAt
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
