using Dfe.Complete.Validators;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators;

public class UrnAttributeTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("12345", false)]
    [InlineData("1234567", false)]
    [InlineData("123456", true)]
    public void UrnAttribute_Validation_ReturnsExpectedResult(string urn, bool expectedIsValid)
    {
        // Arrange
        var attribute = new UrnAttribute();
        var validationContext = new ValidationContext(new { TestUrn = urn }, null, null)
        {
            MemberName = "TestUrn"
        };

        // Act
        var result = attribute.GetValidationResult(urn, validationContext);

        // Assert
        if (expectedIsValid)
        {
            Assert.Null(result);
        }
        else
        {
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
        }
    }
}