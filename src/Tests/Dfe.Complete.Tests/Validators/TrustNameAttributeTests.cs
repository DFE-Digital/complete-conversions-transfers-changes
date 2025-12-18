using Dfe.Complete.Validators;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators;

public class TrustNameAttributeTests
{
    private static TrustNameAttribute CreateAttribute()
    {
        var objectInstance = new { TrustName = "", TrustReference = "" };
        return new TrustNameAttribute(nameof(objectInstance.TrustReference));
    }

    [Fact]
    public void Validation_Fails_When_NoTrustRefAndNoTrustName()
    {
        // Arrange
        var attribute = CreateAttribute();
        var validationContext = new ValidationContext(new { TrustName = "", TrustReference = "" })
        {
            MemberName = "TrustName"
        };

        // Act
        var result = attribute.GetValidationResult("", validationContext);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Enter a trust name.", result.ErrorMessage);
    }

    [Fact]
    public void Validation_Passes_When_NoTrustRefButTrustNameExists()
    {
        // Arrange
        var attribute = CreateAttribute();
        var validationContext = new ValidationContext(new { TrustName = "New trust name", TrustReference = "" })
        {
            MemberName = "TrustName"
        };

        // Act
        var result = attribute.GetValidationResult("New trust name", validationContext);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Validation_Passes_When_TrustRefExistsButTrustNameIsEmpty()
    {
        // Arrange
        var attribute = CreateAttribute();
        var validationContext = new ValidationContext(new { TrustName = "", TrustReference = "TR1234567" })
        {
            MemberName = "TrustName"
        };

        // Act
        var result = attribute.GetValidationResult("", validationContext);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Validation_Passes_When_TrustRefExistsAndTrustNameProvided()
    {
        // Arrange
        var attribute = CreateAttribute();
        var validationContext = new ValidationContext(new { TrustName = "Big new trust", TrustReference = "TR12345" })
        {
            MemberName = "TrustName"
        };

        // Act
        var result = attribute.GetValidationResult("Big new trust", validationContext);

        // Assert
        Assert.Null(result);
    }
}
