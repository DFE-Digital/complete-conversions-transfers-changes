using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Validators;
using MediatR;
using Moq;

namespace Dfe.Complete.Tests.Validators;

public class TrustNameAttributeTests
{
    [Theory]
    [InlineData("", null, "", false, "Enter a trust name.")] // Invalid if no ref or trust name entered
    [InlineData("", null, "New trust name", true, null)] // Valid if no trust reference number but trust name
    [InlineData("TR1234567", null, "", true, null)] // Valid if trust ref exists but trust name is null or empty
    [InlineData("TR12345", "Big new trust", "Big new trust", true, null)] // Valid if trust is found anf trust name matched
    [InlineData("TR00000", null, "Big new trust", true, null)] // Valid if trust ref did not exist previously
    [InlineData("TR12345", "Big new trust", "New trust name", false, "A trust with this TRN already exists. It is called Big new trust. Check the trust name you have entered for this conversion/transfer.")] // Invalid if trust ref exists but name is different
    public void TrustNameAttribute_Validation_ReturnsExpectedResult(
        string trn, string existingTrustName, string trustName, bool isValid, string expectedErrorMessage)
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        
        if (!string.IsNullOrEmpty(trn))
        {
            var trnResult = string.IsNullOrEmpty(existingTrustName) 
                ? Result<GetProjectByTrnResponseDto?>.Success(null)
                : Result<GetProjectByTrnResponseDto?>.Success(new GetProjectByTrnResponseDto(Guid.NewGuid(), existingTrustName));
            
            mockSender.Setup(s => s.Send(It.IsAny<GetProjectByTrnQuery>(), default))
                .ReturnsAsync(trnResult);
        }
        
        var objectInstance = new { TrustName = trustName, TrustReference = trn };
        var attribute = new TrustNameAttribute(nameof(objectInstance.TrustReference), mockSender.Object);
        var validationContext = new ValidationContext(objectInstance, null, null)
        {
            MemberName = nameof(objectInstance.TrustName)
        };

        // Act
        var result = attribute.GetValidationResult(trustName, validationContext);

        // Assert
        if (isValid)
        {
            Assert.Null(result);
        }
        else
        {
            Assert.NotNull(result);
            Assert.Equal(expectedErrorMessage, result.ErrorMessage);
        }
    }
}
