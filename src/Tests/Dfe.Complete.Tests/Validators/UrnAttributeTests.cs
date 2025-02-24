using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Validators;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Moq;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Services.AcademiesApi;

namespace Dfe.Complete.Tests.Validators;

public class UrnAttributeTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("12345", false)]
    [InlineData("1234567", false)]
    [InlineData("123456", false)]
    [InlineData("133456", true)]
    public void UrnAttribute_Validation_ReturnsExpectedResult(string urn, bool expectedIsValid)
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        
        mockSender.Setup(s => s.Send(It.IsAny<GetEstablishmentByUrnRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EstablishmentDto>.Success(new EstablishmentDto()));

        var projectDtoToReturn = urn == "123456" ? new ProjectDto() : null;
        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByUrnQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Success(projectDtoToReturn));

        var attribute = new UrnAttribute();
        var validationContext = new ValidationContext(new { TestUrn = urn }, null, null)
        {
            MemberName = "TestUrn"
        };
        validationContext.InitializeServiceProvider(type =>
            type == typeof(ISender) ? mockSender.Object : null);

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

    [Fact]
    public void UrnAttribute_Validation_Fails_WhenUrnAlreadyExists()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        int urnValue = 123456;

        mockSender.Setup(s => s.Send(It.IsAny<GetEstablishmentByUrnRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EstablishmentDto>.Success(new EstablishmentDto()));

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByUrnQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto?>.Success(new ProjectDto() { Urn = new Urn(urnValue) }));

        var attribute = new UrnAttribute();
        var validationContext = new ValidationContext(new { TestUrn = urnValue.ToString() }, null, null)
        {
            MemberName = "TestUrn"
        };
        validationContext.InitializeServiceProvider(type =>
            type == typeof(ISender) ? mockSender.Object : null);

        // Act
        var result = attribute.GetValidationResult(urnValue.ToString(), validationContext);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ValidationResult>(result);
        Assert.Equal($"A project with the urn: {urnValue} already exists", result.ErrorMessage);
    }

    [Fact]
    public void UrnAttribute_Validation_Returns_ValidationErrorMessage_WhenQueryNotSuccessful()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        int urnValue = 123456;
        var expectedErrorMessage = "Error Message";

        mockSender.Setup(s => s.Send(It.IsAny<GetEstablishmentByUrnRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EstablishmentDto>.Success(new EstablishmentDto()));

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByUrnQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto?>.Failure(expectedErrorMessage));

        var attribute = new UrnAttribute();
        var validationContext = new ValidationContext(new { TestUrn = urnValue.ToString() }, null, null)
        {
            MemberName = "TestUrn"
        };
        validationContext.InitializeServiceProvider(type =>
            type == typeof(ISender) ? mockSender.Object : null);

        // Act
        var result = attribute.GetValidationResult(urnValue.ToString(), validationContext);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ValidationResult>(result);
        Assert.Equal(expectedErrorMessage, result.ErrorMessage);
    }

    [Fact]
    public void UrnAttribute_Validation_RethrowsUnhandledException()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        int urnValue = 123456;
        var unhandledException = new Exception("Unhandled error");

        mockSender.Setup(s => s.Send(It.IsAny<GetEstablishmentByUrnRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EstablishmentDto>.Success(new EstablishmentDto()));

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByUrnQuery>(), It.IsAny<CancellationToken>()))
            .Throws(unhandledException);

        var attribute = new UrnAttribute();
        var validationContext = new ValidationContext(new { TestUrn = urnValue.ToString() }, null, null)
        {
            MemberName = "TestUrn"
        };
        validationContext.InitializeServiceProvider(type =>
            type == typeof(ISender) ? mockSender.Object : null);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() =>
            attribute.GetValidationResult(urnValue.ToString(), validationContext));
        Assert.Equal("Unhandled error", ex.Message);
    }

    [Fact]
    public void UrnAttribute_Validation_Fails_WhenUrnDoesNotExistInAcademies()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        string urnValue = "123456";

        mockSender.Setup(s => s.Send(It.IsAny<GetEstablishmentByUrnRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EstablishmentDto>.Failure("There's no school or academy with that URN. Check the number you entered is correct."));
        
        var attribute = new UrnAttribute();
        var validationContext = new ValidationContext(new { TestUrn = urnValue }, null, null)
        {
            MemberName = "TestUrn"
        };
        validationContext.InitializeServiceProvider(type =>
            type == typeof(ISender) ? mockSender.Object : null);

        // Act
        var result = attribute.GetValidationResult(urnValue, validationContext);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ValidationResult>(result);
        Assert.Equal("There's no school or academy with that URN. Check the number you entered is correct.", result.ErrorMessage);
    }
}