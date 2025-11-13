using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Domain.Tests.Validators
{
    public class UkprnAttributeTests
    {
        [Theory]
        [InlineData(null, true)]
        [InlineData(10000000, true)]
        [InlineData(19999999, true)]
        [InlineData(12345678, true)]
        [InlineData(9999999, false)]
        [InlineData(20000000, false)]
        [InlineData(1234567, false)]
        [InlineData(123456789, false)]
        public void UkprnAttribute_WithInteger_ReturnsExpectedResult(
            int? ukprn,
            bool expectedIsValid)
        {
            // Arrange
            var attribute = new UkprnAttribute() { ValueIsInteger = true };
            var validationContext = new ValidationContext(new { TestUkprn = ukprn });
            validationContext.MemberName = "TestUkprn";

            // Act
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateValue(
                ukprn,
                validationContext,
                validationResults,
                [attribute]);

            // Assert
            Assert.Equal(expectedIsValid, isValid);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(10000000, true)]
        [InlineData(19999999, true)]
        [InlineData(12345678, true)]
        [InlineData(9999999, false)]
        [InlineData(20000000, false)]
        public void UkprnAttribute_WithUkprnObject_ReturnsExpectedResult(
            int? ukprnValue,
            bool expectedIsValid)
        {
            // Arrange
            var attribute = new UkprnAttribute() { ValueIsInteger = false };
            var validationContext = new ValidationContext(new { TestUkprn = ukprnValue });
            validationContext.MemberName = "TestUkprn";
            Ukprn? ukprn = ukprnValue.HasValue ? new Ukprn(ukprnValue.Value) : null;

            // Act
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateValue(
                ukprn,
                validationContext,
                validationResults,
                [attribute]);

            // Assert
            Assert.Equal(expectedIsValid, isValid);
        }

        [Fact]
        public void UkprnAttribute_WithInvalidType_ReturnsFalse()
        {
            // Arrange
            var attribute = new UkprnAttribute() { ValueIsInteger = false };
            var validationContext = new ValidationContext(new { TestUkprn = "invalid" });
            validationContext.MemberName = "TestUkprn";

            // Act
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateValue(
                "12345678",
                validationContext,
                validationResults,
                [attribute]);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void UkprnAttribute_WithIntegerFalse_AndIntegerValue_ReturnsFalse()
        {
            // Arrange
            var attribute = new UkprnAttribute() { ValueIsInteger = false };
            var validationContext = new ValidationContext(new { TestUkprn = 12345678 });
            validationContext.MemberName = "TestUkprn";

            // Act
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateValue(
                12345678,
                validationContext,
                validationResults,
                [attribute]);

            // Assert
            Assert.False(isValid);
        }
    }
}
