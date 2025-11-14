using Dfe.Complete.Infrastructure.Notify;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Dfe.Complete.Application.Tests.Notify
{
    public class NotifyOptionsValidationTests
    {
        [Fact]
        public void NotifyOptions_WithValidConfiguration_PassesValidation()
        {
            // Arrange
            var options = new NotifyOptions
            {
                ApiKey = "test-api-key-12345",
                ServiceId = "test-service-id",
                TestMode = false,
                Email = new NotifyOptions.EmailOptions
                {
                    Templates = new Dictionary<string, string>
                    {
                        { "TestTemplate", "test-template-id" }
                    },
                    DefaultReplyToId = "reply-to-id"
                },
                Retry = new NotifyOptions.RetryOptions
                {
                    MaxRetryAttempts = 3,
                    BaseDelaySeconds = 2
                }
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                options,
                new ValidationContext(options),
                validationResults,
                validateAllProperties: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void NotifyOptions_WithMissingApiKey_FailsValidation()
        {
            // Arrange
            var options = new NotifyOptions
            {
                ApiKey = string.Empty,
                Email = new NotifyOptions.EmailOptions
                {
                    Templates = new Dictionary<string, string>()
                },
                Retry = new NotifyOptions.RetryOptions()
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                options,
                new ValidationContext(options),
                validationResults,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage!.Contains("API key"));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(11)]
        public void RetryOptions_WithInvalidMaxRetryAttempts_FailsValidation(int invalidValue)
        {
            // Arrange
            var retryOptions = new NotifyOptions.RetryOptions
            {
                MaxRetryAttempts = invalidValue,
                BaseDelaySeconds = 2
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                retryOptions,
                new ValidationContext(retryOptions),
                validationResults,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage!.Contains("MaxRetryAttempts"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(61)]
        public void RetryOptions_WithInvalidBaseDelaySeconds_FailsValidation(int invalidValue)
        {
            // Arrange
            var retryOptions = new NotifyOptions.RetryOptions
            {
                MaxRetryAttempts = 3,
                BaseDelaySeconds = invalidValue
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                retryOptions,
                new ValidationContext(retryOptions),
                validationResults,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage!.Contains("BaseDelaySeconds"));
        }

        [Fact]
        public void NotifyOptions_SectionConstant_HasCorrectValue()
        {
            // Assert
            Assert.Equal("Notify", NotifyOptions.Section);
        }

        [Fact]
        public void NotifyOptions_DefaultValues_AreSetCorrectly()
        {
            // Arrange
            var options = new NotifyOptions
            {
                ApiKey = "test-key",
                Email = new NotifyOptions.EmailOptions()
            };

            // Assert
            Assert.False(options.TestMode);
            Assert.Contains("@education.gov.uk", options.TestModeAllowedDomains);
            Assert.Equal(3, options.Retry.MaxRetryAttempts);
            Assert.Equal(2, options.Retry.BaseDelaySeconds);
        }
    }
}

