using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Validators;
using FluentValidation.TestHelper;

namespace Dfe.Complete.Tests.Validators;

public class OtherExternalInputValidatorTests
{
    [Theory]
    [InlineData("TestUser", "HeadTeacher", "schoolacademy", true)]
    [InlineData("TestUser", "HeadTeacher", "schoolacademy", false)]
    [InlineData("TestUser", "HeadTeacher", "incomingtrust", true)]
    [InlineData("TestUser", "HeadTeacher", "incomingtrust", false)]
    [InlineData("TestUser", "HeadTeacher", "outgoingtrust", true)]
    [InlineData("TestUser", "HeadTeacher", "outgoingtrust", false)]
    [InlineData("TestUser", "HeadTeacher", "localauthority", true)]
    [InlineData("TestUser", "HeadTeacher", "localauthority", false)]
    [InlineData("TestUser", "HeadTeacher", "solicitor", false)]
    [InlineData("TestUser", "HeadTeacher", "diocese", false)]
    [InlineData("TestUser", "HeadTeacher", "other", false)]    
    public void Should_Not_Have_Errors_When_Valid_Input(string fullName, string role, string selectedExternalContactType,  bool isPrimaryContact)
    {
        // Arrange       
        var model = new OtherExternalContactInputModel
        {
            FullName = fullName,
            Role = role,
            SelectedExternalContactType = selectedExternalContactType,
            IsPrimaryProjectContact = isPrimaryContact,
        };

        var validator = new OtherExternalContactInputValidator();

        // Act
        var result = validator.TestValidate(model);


        // Assert
        Assert.Multiple(
            () => Assert.True(result.IsValid),
            () => Assert.Empty(result.Errors)
        );
    }

    [Theory]
    [InlineData(null, "HeadTeacher", "schoolacademy", true, "FullName")]
    [InlineData("TestUser", null, "schoolacademy", false, "Role")]    
    [InlineData("TestUser", "HeadTeacher", "solicitor", true, "IsPrimaryProjectContact")]
    [InlineData("TestUser", "HeadTeacher", "diocese", true, "IsPrimaryProjectContact")]
    [InlineData("TestUser", "HeadTeacher", "other", true, "IsPrimaryProjectContact")]
    public void Should_Have_Errors_When_Invalid_Input(string fullName, string role, string selectedExternalContactType, bool isPrimaryContact, string expectedPropertyNameInErrorList)
    {
       
        // Arrange       
        var model = new OtherExternalContactInputModel
        {
            FullName = fullName,
            Role = role,
            SelectedExternalContactType = selectedExternalContactType,
            IsPrimaryProjectContact = isPrimaryContact,
        };

        var validator = new OtherExternalContactInputValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Multiple(
            () => Assert.False(result.IsValid),
            () => Assert.Contains(result.Errors, x => x.PropertyName == expectedPropertyNameInErrorList)
        );
    }
}
