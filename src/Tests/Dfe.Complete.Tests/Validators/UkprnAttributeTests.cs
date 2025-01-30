using System.ComponentModel.DataAnnotations;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Validators;
using Moq;

namespace Dfe.Complete.Tests.Validators
{
    public class UkprnAttributeTests
    {
        [Theory]
        [InlineData(null, false, null, "Enter a UKPRN")]
        [InlineData("", false, null, "Enter a UKPRN")]
        [InlineData("1234567", false, null, "The TestUkprn must be 8 digits long and start with a 1. For example, 12345678.")]
        [InlineData("12345678", false, null, "There's no trust with that UKPRN. Check the number you entered is correct")]
        [InlineData("12345678", true, null, null)]
        [InlineData("12345678", true, "12345678", "The outgoing and incoming trust cannot be the same")]
        public async Task UkprnAttribute_Validation_ReturnsExpectedResult(
            string ukprn,
            bool trustExists,
            string comparisonPropertyValue,
            string expectedErrorMessage)
        {
            // Arrange
            var mockTrustClient = new Mock<ITrustsClient>();

            if (trustExists)
            {
                mockTrustClient
                    .Setup(service => service.GetTrustByUkprnAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new TrustResponse());
            }
            else
            {
                mockTrustClient
                    .Setup(service => service.GetTrustByUkprnAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .Throws(new AggregateException());
            }

            var objectInstance = new
            {
                TestUkprn = ukprn,
                ComparisonUkprn = comparisonPropertyValue
            };

            var attribute = new UkprnAttribute(nameof(objectInstance.ComparisonUkprn));
            var validationContext = new ValidationContext(objectInstance, null, null)
            {
                MemberName = nameof(objectInstance.TestUkprn)
            };
            validationContext.InitializeServiceProvider(type =>
                type == typeof(ITrustsClient) ? mockTrustClient.Object : null);

            // Act
            var result = attribute.GetValidationResult(ukprn, validationContext);

            // Assert
            if (expectedErrorMessage == null)
            {
                Assert.Null(result); // Success returns null
            }
            else
            {
                Assert.NotNull(result);
                Assert.IsType<ValidationResult>(result);
                Assert.Equal(expectedErrorMessage, result.ErrorMessage);
            }
        }
    }
}
