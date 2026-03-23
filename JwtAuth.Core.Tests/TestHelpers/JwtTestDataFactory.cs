using JwtAuth.Core.Models;
using JwtAuth.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace JwtAuth.Core.Tests.TestHelpers
{
    internal static class JwtTestDataFactory
    {
        internal static JwtSettings CreateValidSettings(
            int expiryMinutes = 60,
            string secretKey = "this-is-a-very-long-secret-key-for-tests-12345")
        {
            return new JwtSettings
            {
                SecretKey = secretKey,
                Issuer = "JwtAuthShowcase.Tests",
                Audience = "JwtAuthShowcase.Client",
                ExpiryMinutes = expiryMinutes
            };
        }

        internal static JwtTokenGenerator CreateGenerator(JwtSettings settings)
        {
            return new JwtTokenGenerator(Options.Create(settings));
        }

        internal static IConfiguration CreateConfiguration(JwtSettings settings)
        {
            var values = new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = settings.SecretKey,
                ["JwtSettings:Issuer"] = settings.Issuer,
                ["JwtSettings:Audience"] = settings.Audience,
                ["JwtSettings:ExpiryMinutes"] = settings.ExpiryMinutes.ToString()
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
        }
    }
}
