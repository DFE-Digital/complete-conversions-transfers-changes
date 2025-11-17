using Dfe.Complete.Configuration;
using Dfe.Complete.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Complete.Tests.Services
{
    public class MaintenanceBannerServiceTests
    {
        private readonly Mock<ILogger<MaintenanceBannerService>> _mockLogger;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;

        public MaintenanceBannerServiceTests()
        {
            _mockLogger = new Mock<ILogger<MaintenanceBannerService>>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
        }

        private MaintenanceBannerService CreateService(MaintenanceBannerOptions options)
        {
            var mockOptions = new Mock<IOptions<MaintenanceBannerOptions>>();
            mockOptions.Setup(x => x.Value).Returns(options);
            return new MaintenanceBannerService(mockOptions.Object, _mockLogger.Object, _mockEnvironment.Object);
        }

        [Fact]
        public void ShouldShowBanner_WhenNotProduction_ShouldReturnFalse()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Development);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(1),
                MaintenanceEnd = DateTime.UtcNow.AddHours(3),
                NotifyFrom = DateTime.UtcNow.AddMinutes(-30)
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldShowBanner_WhenProductionButDisabled_ShouldReturnFalse()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = false,
                MaintenanceStart = DateTime.UtcNow.AddHours(1),
                MaintenanceEnd = DateTime.UtcNow.AddHours(3),
                NotifyFrom = DateTime.UtcNow.AddMinutes(-30)
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldShowBanner_WhenMissingDates_ShouldReturnFalseAndLogError()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = null,
                MaintenanceEnd = DateTime.UtcNow.AddHours(3),
                NotifyFrom = DateTime.UtcNow.AddMinutes(-30)
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Maintenance banner configuration is incomplete")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ShouldShowBanner_WhenMaintenanceEndInPast_ShouldReturnFalseAndLogError()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(-3),
                MaintenanceEnd = DateTime.UtcNow.AddHours(-1), // In the past
                NotifyFrom = DateTime.UtcNow.AddHours(-4)
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MaintenanceEnd") && v.ToString()!.Contains("is in the past")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ShouldShowBanner_WhenMaintenanceEndBeforeStart_ShouldReturnFalseAndLogError()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(3),
                MaintenanceEnd = DateTime.UtcNow.AddHours(1), // Before start
                NotifyFrom = DateTime.UtcNow.AddMinutes(-30)
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("is before MaintenanceStart")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ShouldShowBanner_WhenNotifyToBeforeNotifyFrom_ShouldReturnFalseAndLogError()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(2),
                MaintenanceEnd = DateTime.UtcNow.AddHours(4),
                NotifyFrom = DateTime.UtcNow.AddMinutes(-30),
                NotifyTo = DateTime.UtcNow.AddMinutes(-60) // Before NotifyFrom
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("NotifyTo") && v.ToString()!.Contains("must be after NotifyFrom")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ShouldShowBanner_WhenNotifyToIsNull_ShouldDefaultToMaintenanceEnd()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(1),
                MaintenanceEnd = DateTime.UtcNow.AddHours(3),
                NotifyFrom = DateTime.UtcNow.AddMinutes(-30),
                NotifyTo = null // Should default to MaintenanceEnd
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ShouldShowBanner_WhenNotifyToPassed_ShouldReturnFalse()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(1),
                MaintenanceEnd = DateTime.UtcNow.AddHours(3),
                NotifyFrom = DateTime.UtcNow.AddHours(-2),
                NotifyTo = DateTime.UtcNow.AddMinutes(-30) // In the past
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldShowBanner_WhenNotifyTimeNotReached_ShouldReturnFalse()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(2),
                MaintenanceEnd = DateTime.UtcNow.AddHours(4),
                NotifyFrom = DateTime.UtcNow.AddHours(1) // In the future
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldShowBanner_WhenMaintenanceHasEnded_ShouldReturnFalse()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(-3),
                MaintenanceEnd = DateTime.UtcNow.AddHours(-1), // In the past but after start
                NotifyFrom = DateTime.UtcNow.AddHours(-4)
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldShowBanner_WhenValidConditions_ShouldReturnTrue()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(Environments.Production);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(1),
                MaintenanceEnd = DateTime.UtcNow.AddHours(3),
                NotifyFrom = DateTime.UtcNow.AddMinutes(-30) // Started notifying
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeTrue();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Maintenance banner check")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ShouldShowBanner_WhenExceptionThrown_ShouldReturnFalseAndLogError()
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Throws(new Exception("Test exception"));
            var options = new MaintenanceBannerOptions { Enabled = true };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error checking if maintenance banner should be shown")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void GetBannerMessage_WhenCustomMessageProvided_ShouldReturnCustomMessage()
        {
            // Arrange
            var customMessage = "Custom maintenance message";
            var options = new MaintenanceBannerOptions
            {
                Message = customMessage
            };
            var service = CreateService(options);

            // Act
            var result = service.GetBannerMessage();

            // Assert
            result.Should().Be(customMessage);
        }

        [Fact]
        public void GetBannerMessage_WhenNoDatesProvided_ShouldReturnDefaultMessage()
        {
            // Arrange
            var options = new MaintenanceBannerOptions
            {
                Message = string.Empty,
                MaintenanceStart = null,
                MaintenanceEnd = null
            };
            var service = CreateService(options);

            // Act
            var result = service.GetBannerMessage();

            // Assert
            result.Should().Be("The Complete conversions, transfers and changes service will be temporarily unavailable due to scheduled maintenance work.");
        }

        [Fact]
        public void GetBannerMessage_WhenDatesProvided_ShouldReturnFormattedMessage()
        {
            // Arrange
            var startDate = new DateTime(2025, 11, 19, 16, 0, 0, DateTimeKind.Utc);
            var endDate = new DateTime(2025, 11, 19, 18, 0, 0, DateTimeKind.Utc);
            var options = new MaintenanceBannerOptions
            {
                Message = string.Empty,
                MaintenanceStart = startDate,
                MaintenanceEnd = endDate
            };
            var service = CreateService(options);

            // Act
            var result = service.GetBannerMessage();

            // Assert
            result.Should().StartWith("The Complete conversions, transfers and changes service will be unavailable from");
            result.Should().Contain("until");
            result.Should().EndWith("due to scheduled maintenance work.");
        }

        [Fact]
        public void GetBannerMessage_WhenEmptyMessage_ShouldGenerateDefault()
        {
            // Arrange
            var startDate = new DateTime(2025, 11, 19, 16, 0, 0, DateTimeKind.Utc);
            var endDate = new DateTime(2025, 11, 19, 18, 0, 0, DateTimeKind.Utc);
            var options = new MaintenanceBannerOptions
            {
                Message = "   ", // Whitespace only
                MaintenanceStart = startDate,
                MaintenanceEnd = endDate
            };
            var service = CreateService(options);

            // Act
            var result = service.GetBannerMessage();

            // Assert
            result.Should().StartWith("The Complete conversions, transfers and changes service will be unavailable from");
        }

        [Fact]
        public void GetConfiguration_ShouldReturnOptions()
        {
            // Arrange
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow,
                MaintenanceEnd = DateTime.UtcNow.AddHours(2),
                NotifyFrom = DateTime.UtcNow.AddMinutes(-30),
                Message = "Test message"
            };
            var service = CreateService(options);

            // Act
            var result = service.GetConfiguration();

            // Assert
            result.Should().BeSameAs(options);
        }

        [Theory]
        [InlineData("Development")]
        [InlineData("Staging")]
        [InlineData("Test")]
        public void ShouldShowBanner_WhenNonProductionEnvironment_ShouldAlwaysReturnFalse(string environment)
        {
            // Arrange
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns(environment);
            var options = new MaintenanceBannerOptions
            {
                Enabled = true,
                MaintenanceStart = DateTime.UtcNow.AddHours(1),
                MaintenanceEnd = DateTime.UtcNow.AddHours(3),
                NotifyFrom = DateTime.UtcNow.AddMinutes(-30)
            };
            var service = CreateService(options);

            // Act
            var result = service.ShouldShowBanner();

            // Assert
            result.Should().BeFalse();
        }
    }
}