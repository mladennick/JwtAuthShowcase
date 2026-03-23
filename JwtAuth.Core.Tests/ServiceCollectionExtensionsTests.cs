using JwtAuth.Core.Extensions;
using JwtAuth.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using JwtAuth.Core.Tests.TestHelpers;

namespace JwtAuth.Core.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddCustomJwtAuthentication_WithValidConfiguration_RegistersTokenGenerator()
        {
            var services = new ServiceCollection();
            var configuration = JwtTestDataFactory.CreateConfiguration(JwtTestDataFactory.CreateValidSettings());

            services.AddCustomJwtAuthentication(configuration);

            var provider = services.BuildServiceProvider();
            var service = provider.GetService<IJwtTokenGenerator>();
            Assert.NotNull(service);
        }

        [Fact]
        public void AddCustomJwtAuthentication_WithoutSecretKey_ThrowsInvalidOperationException()
        {
            var services = new ServiceCollection();
            var settings = JwtTestDataFactory.CreateValidSettings();
            settings.SecretKey = string.Empty;
            var configuration = JwtTestDataFactory.CreateConfiguration(settings);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                services.AddCustomJwtAuthentication(configuration));

            Assert.Equal("JwtSettings:SecretKey is required.", exception.Message);
        }
    }
}
