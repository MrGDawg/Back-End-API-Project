using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UserManagementAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generate a signed JWT for the provided username using configuration
        /// values from the "Jwt" section (Issuer, Audience, Key, ExpiresInMinutes).
        /// </summary>
        /// <param name="username">The subject for the token (username).</param>
        /// <returns>A signed JWT as a compact serialized string.</returns>
        public string GenerateToken(string username)
        {
            // Build JWT using configuration. The key should be a sufficiently
            // random secret stored outside of source control (appsettings.* for
            // local development; use secrets manager or environment variables
            // in CI/CD and production).
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims include subject and a unique identifier (JTI). Add roles or
            // additional claims as needed but avoid storing sensitive data.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(jwtSettings["ExpiresInMinutes"]!)
                ),
                signingCredentials: creds
            );

            // Return serialized token. Consumers should treat this token as an
            // opaque credential and never log it in plain text.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
