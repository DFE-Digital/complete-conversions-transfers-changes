using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Validators;

namespace Dfe.Complete.Tests.Validators;

public class TrnAttributeTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("12345", false)]
    [InlineData("TRABCDE", false)]
    [InlineData("TR12345", true)]
    [InlineData("TR00000", true)]
    public void TrnAttribute_Validation_ReturnsExpectedResult(string trn, bool isValid)
    {
        // Arrange
        var objectInstance = new { TestTrn = trn };
        var attribute = new TrnAttribute();
        var validationContext = new ValidationContext(objectInstance, null, null)
        {
            MemberName = nameof(objectInstance.TestTrn)
        };
        
        // Act
        var result = attribute.GetValidationResult(trn, validationContext);
        
        // Assert
        if (isValid)
        {
            Assert.Null(result); // Success returns null
        }
        else
        {
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Equal("The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234", result.ErrorMessage);
        }
    }
}