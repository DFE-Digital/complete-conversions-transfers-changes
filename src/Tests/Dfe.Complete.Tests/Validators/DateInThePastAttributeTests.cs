using Dfe.Complete.Validators;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators
{
    public class DateInThePastAttributeTests
    {
        [Fact]
        public void DateInThePastAttribute_Validation_ReturnsError_WhenDateInTheFuture()
        {
            // Arrange
            var attribute = new DateInPastAttribute();
            var validationContext = new ValidationContext(new { }, null, null)
            {
                MemberName = "TestDateInThePast",
            };

            var date = DateTime.Now.AddDays(1);

            // Act
            var result = attribute.GetValidationResult(date, validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.True(result.ErrorMessage == "The TestDateInThePast date must be in the past.");
        }
        
        [Fact]
        public void DateInThePastAttribute_Validation_ReturnsNull_WhenDateInThePast()
        {
            // Arrange
            var attribute = new DateInPastAttribute();
            var validationContext = new ValidationContext(new { }, null, null)
            {
                MemberName = "TestDateInThePast",
            };

            var date = DateTime.Now.AddDays(-1);

            // Act
            var result = attribute.GetValidationResult(date, validationContext);

            // Assert
            Assert.Null(result);
        }
        
        
        [Fact]
        public void DateInThePastAttribute_Validation_ReturnsNull_WhenDateNotGiven()
        {
            // Arrange
            var attribute = new DateInPastAttribute();
            var validationContext = new ValidationContext(new { }, null, null)
            {
                MemberName = "TestDateInThePast",
            };

            // Act
            var result = attribute.GetValidationResult(string.Empty, validationContext);

            // Assert
            Assert.Null(result);
        }

        
    }
}