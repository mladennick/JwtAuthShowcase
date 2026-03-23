using JwtAuth.Core.Models;
using JwtAuth.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JwtAuth.Core.Extensions
{
    /// <summary>
    /// Provides dependency injection extensions for configuring JWT authentication services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers JWT settings, token generation services, and JWT Bearer authentication validation.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configuration">The application configuration containing the <c>JwtSettings</c> section.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when required JWT settings are missing from configuration.
        /// </exception>
        public static IServiceCollection AddCustomJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind the settings from the API's appsettings.json to the defined model
            var jwtSettingsSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSettingsSection);

            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            if (jwtSettings is null)
            {
                throw new InvalidOperationException("JwtSettings section is missing from configuration.");
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("JwtSettings:SecretKey is required.");
            }

            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            // Register the Token Generator so the API can inject it
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Configure the actual JWT Bearer validation rules
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            return services;
        }
    }
}
