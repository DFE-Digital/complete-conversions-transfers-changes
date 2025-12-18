using Dfe.Complete.Validators;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators
{
    public class UkprnAttributeTests
    {
        [Theory]
        [InlineData(null, null, "Enter a UKPRN")]
        [InlineData("", null, "Enter a UKPRN")]
        [InlineData("1234567", null, "The TestUkprn must be 8 digits long and start with a 1. For example, 12345678.")]
        [InlineData("12345678", null, null)]
        [InlineData("12345678", "12345678", "The outgoing and incoming trust cannot be the same")]
        public async Task UkprnAttribute_Validation_ReturnsExpectedResult(
            string ukprn,
            string comparisonPropertyValue,
            string expectedErrorMessage)
        {
            // Arrange
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
