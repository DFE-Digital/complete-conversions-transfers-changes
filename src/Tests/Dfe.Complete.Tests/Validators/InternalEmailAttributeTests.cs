using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Validators;

namespace Dfe.Complete.Tests.Validators;

public class InternalEmailAttributeTests
{
    [Theory]
    [InlineData("nicholas.warms@education.gov.uk",  true)]  // Valid email
    [InlineData("@education.gov.uk", false)]  // no prefix
    [InlineData("35", false)] // Number above range
    [InlineData("abc", false)] // Non-numeric string
    [InlineData("", true)]    // Empty string
    [InlineData(null, true)]  // Null value
    public void InternalEmailAttribute_Validation_WorksAsExpected(string value, bool expectedIsValid)
    {
        // Arrange
        var attribute = new InternalEmailAttribute();
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