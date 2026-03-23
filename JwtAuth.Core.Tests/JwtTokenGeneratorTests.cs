using JwtAuth.Core.Models;
using JwtAuth.Core.Services;
using JwtAuth.Core.Tests.TestHelpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text;

namespace JwtAuth.Core.Tests
{
    public class JwtTokenGeneratorTests
    {
        [Fact]
        public void GenerateToken_WithValidInputs_ReturnsJwtContainingExpectedClaims()
        {
            var settings = JwtTestDataFactory.CreateValidSettings();
            var generator = JwtTestDataFactory.CreateGenerator(settings);
            var userId = Guid.NewGuid();
            const string username = "test-user";

            var token = generator.GenerateToken(userId, username);

            Assert.False(string.IsNullOrWhiteSpace(token));

            var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            Assert.Equal(settings.Issuer, parsedToken.Issuer);
            Assert.Contains(settings.Audience, parsedToken.Audiences);
            Assert.Equal(userId.ToString(), parsedToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal(username, parsedToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
            Assert.False(string.IsNullOrWhiteSpace(parsedToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value));
        }

        [Fact]
        public void GenerateToken_WithOneMinuteExpiry_SetsExpirationCloseToExpectedTime()
        {
            var settings = JwtTestDataFactory.CreateValidSettings(expiryMinutes: 1);
            var generator = JwtTestDataFactory.CreateGenerator(settings);
            var beforeGeneration = DateTime.UtcNow;

            var token = generator.GenerateToken(Guid.NewGuid(), "exp-user");
            var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var expectedMin = beforeGeneration.AddSeconds(50);
            var expectedMax = beforeGeneration.AddSeconds(70);
            Assert.InRange(parsedToken.ValidTo, expectedMin, expectedMax);
        }

        [Fact]
        public void GenerateToken_WithValidSettings_ProducesTokenThatPassesSignatureValidation()
        {
            var settings = JwtTestDataFactory.CreateValidSettings();
            var generator = JwtTestDataFactory.CreateGenerator(settings);
            var token = generator.GenerateToken(Guid.NewGuid(), "signed-user");
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            Assert.NotNull(principal);
            Assert.NotNull(validatedToken);
            Assert.IsType<JwtSecurityToken>(validatedToken);
        }

        [Fact]
        public void GenerateToken_WhenValidatedWithWrongSecret_ThrowsSignatureValidationException()
        {
            var settings = JwtTestDataFactory.CreateValidSettings();
            var generator = JwtTestDataFactory.CreateGenerator(settings);
            var token = generator.GenerateToken(Guid.NewGuid(), "wrong-secret-user");
            var tokenHandler = new JwtSecurityTokenHandler();

            var wrongSecretValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-a-different-and-invalid-secret-key-99999")),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var exception = Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() =>
                tokenHandler.ValidateToken(token, wrongSecretValidationParameters, out _));

            Assert.False(string.IsNullOrWhiteSpace(exception.Message));
        }

        [Fact]
        public void GenerateToken_WhenPayloadIsTampered_ThrowsSignatureValidationException()
        {
            var settings = JwtTestDataFactory.CreateValidSettings();
            var generator = JwtTestDataFactory.CreateGenerator(settings);
            var token = generator.GenerateToken(Guid.NewGuid(), "original-user");
            var tamperedToken = TamperTokenPayloadUsername(token, "attacker-user");
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var exception = Record.Exception(() =>
                tokenHandler.ValidateToken(tamperedToken, validationParameters, out _));

            Assert.NotNull(exception);
            Assert.True(
                exception is SecurityTokenInvalidSignatureException or SecurityTokenSignatureKeyNotFoundException,
                $"Unexpected exception type: {exception.GetType().FullName}");
            Assert.False(string.IsNullOrWhiteSpace(exception.Message));
        }

        [Theory]
        [InlineData("", "issuer", "audience", 60, "JwtSettings:SecretKey is required.")]
        [InlineData("this-is-a-very-long-secret-key-for-tests-12345", "", "audience", 60, "JwtSettings:Issuer is required.")]
        [InlineData("this-is-a-very-long-secret-key-for-tests-12345", "issuer", "", 60, "JwtSettings:Audience is required.")]
        [InlineData("this-is-a-very-long-secret-key-for-tests-12345", "issuer", "audience", 0, "JwtSettings:ExpiryMinutes must be greater than zero.")]
        public void Constructor_WithInvalidSettings_ThrowsInvalidOperationException(
            string secretKey,
            string issuer,
            string audience,
            int expiryMinutes,
            string expectedMessage)
        {
            var settings = JwtTestDataFactory.CreateValidSettings(expiryMinutes: expiryMinutes);
            settings.SecretKey = secretKey;
            settings.Issuer = issuer;
            settings.Audience = audience;

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                JwtTestDataFactory.CreateGenerator(settings);
            });
            Assert.Equal(expectedMessage, exception.Message);
        }

        private static string TamperTokenPayloadUsername(string token, string newUsername)
        {
            var parts = token.Split('.');
            Assert.Equal(3, parts.Length);

            var payloadBytes = Base64UrlEncoder.DecodeBytes(parts[1]);
            var payloadJson = Encoding.UTF8.GetString(payloadBytes);
            var payloadData = JsonSerializer.Deserialize<Dictionary<string, object?>>(payloadJson)!;
            payloadData[JwtRegisteredClaimNames.UniqueName] = newUsername;
            var modifiedPayloadJson = JsonSerializer.Serialize(payloadData);
            var modifiedPayloadBase64 = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(modifiedPayloadJson));

            return $"{parts[0]}.{modifiedPayloadBase64}.{parts[2]}";
        }
    }
}
