using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Validators;
using MediatR;
using Moq;

namespace Dfe.Complete.Tests.Validators;

public class TrnAttributeTests
{
    [Theory]
    [InlineData(null, false, null, "Enter a Trust reference number (TRN)")]
    [InlineData("", false, null, "Enter a Trust reference number (TRN)")]
    [InlineData("12345", false, null, "The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234")]
    [InlineData("TRABCDE", false, null, "The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234")]
    [InlineData("TR12345", true, null, null)]
    [InlineData("TR12345", true, "Existing Trust", "A trust with this TRN already exists. It is called Existing Trust. Check the trust name you have entered for this conversion/transfer")]
    public async Task TrnAttribute_Validation_ReturnsExpectedResult(
        string trn,
        bool projectExists,
        string existingTrustName,
        string expectedErrorMessage)
    {
        // Arrange
        var mockSender = new Mock<ISender>();

        if (projectExists)
        {
            mockSender.Setup(s => s.Send(It.IsAny<GetProjectByTrnQuery>(), default))
                .ReturnsAsync(Result<GetProjectByTrnResponseDto?>.Success(new GetProjectByTrnResponseDto(Guid.NewGuid(), existingTrustName)));
        }
        else
        {
            mockSender.Setup(s => s.Send(It.IsAny<GetProjectByTrnQuery>(), default))
                .ReturnsAsync(Result<GetProjectByTrnResponseDto?>.Success(null));
        }

        var objectInstance = new { TestTrn = trn };
        var attribute = new TrnAttribute();
        var validationContext = new ValidationContext(objectInstance, null, null)
        {
            MemberName = nameof(objectInstance.TestTrn)
        };
        validationContext.InitializeServiceProvider(type => type == typeof(ISender) ? mockSender.Object : null);

        // Act
        var result = attribute.GetValidationResult(trn, validationContext);

        // Assert
        if (expectedErrorMessage == null)
        {
            Assert.Null(result); // Success returns null
        }
        else
        {
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Equal(expectedErrorMessage, result.ErrorMessage);
        }
    }


    [Fact]
    public async Task TrnAttribute_ShouldReturnValidationMessageError_WhenQueryFails()
    {
        // Arrange
        const string expectedErrorMessage = "Project not found";
        
        var trn = "TR12345";
        
        var mockSender = new Mock<ISender>();

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByTrnQuery>(), default))
            .ReturnsAsync(Result<GetProjectByTrnResponseDto?>.Failure(expectedErrorMessage));

        var objectInstance = new { TestTrn = trn };
        var attribute = new TrnAttribute();
        var validationContext = new ValidationContext(objectInstance, null, null)
        {
            MemberName = nameof(objectInstance.TestTrn)
        };
        validationContext.InitializeServiceProvider(type => type == typeof(ISender) ? mockSender.Object : null);

        // Act & Assert
        var result = attribute.GetValidationResult(trn, validationContext);

        Assert.NotNull(result);
        Assert.Equal(expectedErrorMessage, result.ErrorMessage);
    }
    
    [Fact]
    public void TrnAttribute_Validation_RethrowsUnhandledException()
    {
        // Arrange
        var trn = "TR12345";
        var mockSender = new Mock<ISender>();
        var unhandledException = new Exception("Unhandled error");

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByTrnQuery>(), default))
            .Throws(unhandledException);

        var objectInstance = new { TestTrn = trn };
        var attribute = new TrnAttribute();
        var validationContext = new ValidationContext(objectInstance, null, null)
        {
            MemberName = nameof(objectInstance.TestTrn)
        };
        validationContext.InitializeServiceProvider(type => type == typeof(ISender) ? mockSender.Object : null);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => attribute.GetValidationResult(trn, validationContext));
        Assert.Equal("Unhandled error", ex.Message);
    }
}