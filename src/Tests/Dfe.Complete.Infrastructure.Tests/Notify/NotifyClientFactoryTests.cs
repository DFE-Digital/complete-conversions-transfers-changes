using Dfe.Complete.Infrastructure.Notify;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dfe.Complete.Infrastructure.Tests.Notify
{
    /// <summary>
    /// Tests for NotifyClientFactory
    /// Validates client creation and configuration handling
    /// </summary>
    public class NotifyClientFactoryTests
    {
        [Fact]
        public void Constructor_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new NotifyClientFactory(null!));

            Assert.Equal("options", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithValidOptions_CreatesFactory()
        {
            // Arrange
            var options = Options.Create(new NotifyOptions
            {
                ApiKey = "test-api-key-12345678-1234-1234-1234-123456789012-12345678-1234-1234-1234-123456789012"
            });

            // Act
            var factory = new NotifyClientFactory(options);

            // Assert
            Assert.NotNull(factory);
        }

        [Fact]
        public void CreateClient_WithValidApiKey_ReturnsClient()
        {
            // Arrange
            var options = Options.Create(new NotifyOptions
            {
                ApiKey = "test-api-key-12345678-1234-1234-1234-123456789012-12345678-1234-1234-1234-123456789012"
            });
            var factory = new NotifyClientFactory(options);

            // Act
            var client = factory.CreateClient();

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public void CreateClient_WithNullApiKey_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = Options.Create(new NotifyOptions
            {
                ApiKey = null!
            });
            var factory = new NotifyClientFactory(options);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                factory.CreateClient());

            Assert.Contains("Notify API key is not configured", exception.Message);
        }

        [Fact]
        public void CreateClient_WithEmptyApiKey_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = Options.Create(new NotifyOptions
            {
                ApiKey = ""
            });
            var factory = new NotifyClientFactory(options);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                factory.CreateClient());

            Assert.Contains("Notify API key is not configured", exception.Message);
        }

        [Fact]
        public void CreateClient_WithWhitespaceApiKey_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = Options.Create(new NotifyOptions
            {
                ApiKey = "   "
            });
            var factory = new NotifyClientFactory(options);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                factory.CreateClient());

            Assert.Contains("Notify API key is not configured", exception.Message);
        }

        [Fact]
        public void CreateClient_CalledMultipleTimes_ReturnsNewInstanceEachTime()
        {
            // Arrange
            var options = Options.Create(new NotifyOptions
            {
                ApiKey = "test-api-key-12345678-1234-1234-1234-123456789012-12345678-1234-1234-1234-123456789012"
            });
            var factory = new NotifyClientFactory(options);

            // Act
            var client1 = factory.CreateClient();
            var client2 = factory.CreateClient();

            // Assert
            Assert.NotNull(client1);
            Assert.NotNull(client2);
            Assert.NotSame(client1, client2); // Different instances
        }
    }
}

