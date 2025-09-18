using Dfe.Complete.Constants;
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
            FullName = "Test User",
            Email = "TestUser@test.com"
        };

        var validator = new ExternalContactInputValidator<ExternalContactInputModel>();

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

        var validator = new ExternalContactInputValidator<ExternalContactInputModel>();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Multiple(
            () => Assert.False(result.IsValid),
            () => Assert.Contains(result.Errors, x => x.PropertyName == "FullName")
        );

    }

    [Fact]
    public void Should_Have_Errors_When_Email_Is_Invalid()
    {

        // Arrange       
        var model = new ExternalContactInputModel
        {
            Email = "TestUser"
        };

        var validator = new ExternalContactInputValidator<ExternalContactInputModel>();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Multiple(
            () => Assert.False(result.IsValid),
            () => Assert.Contains(result.Errors, x => x.PropertyName == "Email")
        );

    }    

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("+44 7123 456 789")]
    [InlineData("07123 456789")]
    [InlineData("07123456789")]
    [InlineData("(07123) 456789")]
    [InlineData("+44 1234 567 890")]
    [InlineData("+441234567890")]
    [InlineData("01234 567890")]
    [InlineData("01234567890")]
    [InlineData("020 8327 3737")]
    public void Should_Not_Have_Errors_When_Phone_Is_Valid(string? phoneNumber)
    {
        // Arrange       
        var model = new ExternalContactInputModel
        {   
            Phone =  phoneNumber
        };

        var validator = new ExternalContactInputValidator<ExternalContactInputModel>();

        // Act
        var result = validator.TestValidate(model);
        
        // Assert
        Assert.Multiple(            
            () => Assert.DoesNotContain(result.Errors, x => x.PropertyName == "Phone")
        );
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("123456")]
    [InlineData("INVALID")]
    [InlineData("07abc123456")]
    public void Should_Not_Have_Errors_When_Phone_Is_InValid(string phoneNumber)
    {
        // Arrange       
        var model = new ExternalContactInputModel
        {
            Phone = phoneNumber
        };

        var validator = new ExternalContactInputValidator<ExternalContactInputModel>();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Multiple(
            () => Assert.False(result.IsValid),
            () => Assert.Contains(result.Errors, x => x.PropertyName == "Phone")
        );
    }    
}
