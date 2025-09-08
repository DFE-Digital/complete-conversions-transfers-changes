using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Validators;
using FluentValidation.TestHelper;

namespace Dfe.Complete.Tests.Validators;

public class ExternalInputValidatorTests
{
    [Fact]
    public void Should_Not_Have_Errors_When_Valid_Input()
    {
        // Arrange       
        var model = new ExternalContactInputModel
        {
            FullName ="Test User",
            Email = "TestUser@test.com"
        };

        var validator = new ExternalContactInputValidator();

        // Act
        var result = validator.TestValidate(model);


        // Assert
        Assert.Multiple(
            () => Assert.True(result.IsValid),
            () => Assert.Empty(result.Errors)
        );
    }

    [Fact]
    public void Should_Have_Errors_When_FullName_Is_Empty()
    {
       
        // Arrange       
        var model = new ExternalContactInputModel
        {  
            Email = "TestUser@test.com"
        };

        var validator = new ExternalContactInputValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Multiple(
            () => Assert.False(result.IsValid),
            () => Assert.Contains(result.Errors, x => x.PropertyName == "FullName")
        );

    }
}
