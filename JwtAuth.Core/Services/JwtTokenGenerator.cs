using JwtAuth.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuth.Core.Services
{
    /// <summary>
    /// Default implementation of <see cref="IJwtTokenGenerator"/> that issues HMAC SHA-256 signed JWT tokens.
    /// </summary>
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenGenerator"/> class.
        /// </summary>
        /// <param name="jwtOptions">JWT settings options bound from configuration.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the configured JWT settings are missing required values or contain invalid data.
        /// </exception>
        public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
            ValidateSettings(_jwtSettings);
        }

        /// <inheritdoc />
        public string GenerateToken(Guid userId, string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validates the configured JWT settings before token generation can occur.
        /// </summary>
        /// <param name="settings">The JWT settings instance to validate.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when one or more required settings are missing or invalid.
        /// </exception>
        private static void ValidateSettings(JwtSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SecretKey))
            {
                throw new InvalidOperationException("JwtSettings:SecretKey is required.");
            }

            if (string.IsNullOrWhiteSpace(settings.Issuer))
            {
                throw new InvalidOperationException("JwtSettings:Issuer is required.");
            }

            if (string.IsNullOrWhiteSpace(settings.Audience))
            {
                throw new InvalidOperationException("JwtSettings:Audience is required.");
            }

            if (settings.ExpiryMinutes <= 0)
            {
                throw new InvalidOperationException("JwtSettings:ExpiryMinutes must be greater than zero.");
            }
        }
    }
}
