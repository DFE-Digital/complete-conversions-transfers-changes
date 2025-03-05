using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Models;

namespace Dfe.Complete.Tests.Validators
{
    public class ValidNumberAttributeTests
    {
        [Theory]
        [InlineData("25", 10, 30, true)]  // Valid number in range
        [InlineData("5", 10, 30, false)]  // Number below range
        [InlineData("35", 10, 30, false)] // Number above range
        [InlineData("abc", 10, 30, false)] // Non-numeric string
        [InlineData("", 10, 30, true)]    // Empty string
        [InlineData(null, 10, 30, true)]  // Null value
        public void ValidNumberAttribute_Validation_WorksAsExpected(string value, int minValue, int maxValue, bool expectedIsValid)
        {
            // Arrange
            var attribute = new ValidNumberAttribute(minValue, maxValue);
            var validationContext = new ValidationContext(new { }); 
            var validationResults = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateValue(
                value,
                validationContext,
                validationResults,
                new List<ValidationAttribute> { attribute });

            // Assert
            Assert.Equal(expectedIsValid, isValid);
        }
    }
}
