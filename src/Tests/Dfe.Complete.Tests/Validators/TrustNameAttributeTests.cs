using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Validators;
using MediatR;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators;

public class TrustNameAttributeTests
{
    private static TrustNameAttribute CreateAttribute(Mock<ISender> mockSender)
    {
        var objectInstance = new { TrustName = "", TrustReference = "" };
        return new TrustNameAttribute(nameof(objectInstance.TrustReference), mockSender.Object);
    }

    [Fact]
    public void Validation_Fails_When_NoTrustRefAndNoTrustName()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        var attribute = CreateAttribute(mockSender);
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
        var mockSender = new Mock<ISender>();
        var attribute = CreateAttribute(mockSender);
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
        var mockSender = new Mock<ISender>();
        var trnResult = Result<GetProjectByTrnResponseDto?>.Success(null);
        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByTrnQuery>(), default)).ReturnsAsync(trnResult);
        var attribute = CreateAttribute(mockSender);
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
    public void Validation_Passes_When_TrustRefExistsAndTrustNameMatches()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        var trnResult = Result<GetProjectByTrnResponseDto?>.Success(new GetProjectByTrnResponseDto(Guid.NewGuid(), "Big new trust"));
        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByTrnQuery>(), default)).ReturnsAsync(trnResult);
        var attribute = CreateAttribute(mockSender);
        var validationContext = new ValidationContext(new { TrustName = "Big new trust", TrustReference = "TR12345" })
        {
            MemberName = "TrustName"
        };

        // Act
        var result = attribute.GetValidationResult("Big new trust", validationContext);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Validation_Passes_When_TrustRefIsNew()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        var trnResult = Result<GetProjectByTrnResponseDto?>.Success(null);
        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByTrnQuery>(), default)).ReturnsAsync(trnResult);
        var attribute = CreateAttribute(mockSender);
        var validationContext = new ValidationContext(new { TrustName = "Big new trust", TrustReference = "TR00000" })
        {
            MemberName = "TrustName"
        };

        // Act
        var result = attribute.GetValidationResult("Big new trust", validationContext);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Validation_Fails_When_TrustRefExistsButTrustNameDiffers()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        var trnResult = Result<GetProjectByTrnResponseDto?>.Success(new GetProjectByTrnResponseDto(Guid.NewGuid(), "Big new trust"));
        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByTrnQuery>(), default)).ReturnsAsync(trnResult);
        var attribute = CreateAttribute(mockSender);
        var validationContext = new ValidationContext(new { TrustName = "New trust name", TrustReference = "TR12345" })
        {
            MemberName = "TrustName"
        };

        // Act
        var result = attribute.GetValidationResult("New trust name", validationContext);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("A trust with this TRN already exists. It is called Big new trust. Check the trust name you have entered for this conversion/transfer.", result.ErrorMessage);
    }
}
