using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Validators;

namespace Dfe.Complete.Tests.Validators
{
    public class ValidNumberAttributeTests
    {
        private class TestModel
        {
            public bool? DependentFlag { get; set; }
            public string? NumberValue { get; set; }
        }

        [Theory]
        [InlineData("25", 10, 30, true)]
        [InlineData("5", 10, 30, false)]
        [InlineData("35", 10, 30, false)]
        [InlineData("abc", 10, 30, false)] 
        [InlineData("", 10, 30, true)]  
        [InlineData(null, 10, 30, true)] 
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
                [attribute]);

            // Assert
            Assert.Equal(expectedIsValid, isValid);
        }

        [Theory]
        [InlineData("25", 10, 30, true, true)]
        [InlineData("25", 10, 30, true, false)]
        [InlineData("abc", 10, 30, true, true)]
        [InlineData("abc", 10, 30, false, false)]
        [InlineData("", 10, 30, true, true)]   
        [InlineData(null, 10, 30, true, true)]  
        public void ValidNumberAttributeWithDepentableFlag_Validation_WorksAsExpected(
                string value,
                int minValue,
                int maxValue,
                bool expectedIsValid,
                bool dependentFlag)
        {
            // Arrange
            var model = new TestModel
            {
                DependentFlag = dependentFlag,
                NumberValue = value
            };

            var attribute = new ValidNumberAttribute(minValue, maxValue, nameof(TestModel.DependentFlag));
            var validationContext = new ValidationContext(model)
            {
                MemberName = nameof(TestModel.NumberValue)
            };

            // Act
            var result = attribute.GetValidationResult(value, validationContext);

            // Assert
            Assert.Equal(expectedIsValid, result == ValidationResult.Success);
        }

    }
}
