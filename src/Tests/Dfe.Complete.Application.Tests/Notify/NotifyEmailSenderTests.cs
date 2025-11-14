using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Infrastructure.Notify;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Notify.Interfaces;
using Xunit;

namespace Dfe.Complete.Application.Tests.Notify
{
    /// <summary>
    /// Unit tests for NotifyEmailSender.
    /// Note: These tests focus on configuration validation and template resolution.
    /// Integration tests with a fake Notify client are in App TemplateIdProviderTests.
    /// </summary>
    public class NotifyEmailSenderTests
    {
        [Fact]
        public void NotifyEmailSender_RequiresNotifyClient()
        {
            // Arrange
            var templateProviderMock = new Mock<ITemplateIdProvider>();
            var loggerMock = new Mock<ILogger<NotifyEmailSender>>();
            var options = Options.Create(new NotifyOptions
            {
                ApiKey = "test-key",
                Email = new NotifyOptions.EmailOptions { Templates = new Dictionary<string, string>() }
            });

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new NotifyEmailSender(null!, templateProviderMock.Object, loggerMock.Object, options));
        }

        [Fact]
        public void NotifyEmailSender_RequiresTemplateProvider()
        {
            // Arrange
            var notifyClientMock = new Mock<IAsyncNotificationClient>();
            var loggerMock = new Mock<ILogger<NotifyEmailSender>>();
            var options = Options.Create(new NotifyOptions
            {
                ApiKey = "test-key",
                Email = new NotifyOptions.EmailOptions { Templates = new Dictionary<string, string>() }
            });

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new NotifyEmailSender(notifyClientMock.Object, null!, loggerMock.Object, options));
        }
    }
}

