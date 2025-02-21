using System.Security.Claims;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.MatConversion;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dfe.Complete.Tests.Pages.Projects.Create.MatConversion;

public class CreateNewProjectModelTests
{
    [Fact]
    public async Task OnPost_ValidModel_ReturnsRedirectResult()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        var mockErrorService = new Mock<ErrorService>();
        var mockLogger = new Mock<ILogger<CreateNewProject>>();

        var projectId = new ProjectId(Guid.NewGuid());
        mockSender
            .Setup(s => s.Send(It.IsAny<CreateMatConversionProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        var pageModel = new CreateNewProject(mockSender.Object, mockErrorService.Object, mockLogger.Object)
        {
            // Set the required bind properties.
            URN = "123456",
            TrustReferenceNumber = "TR12345",
            TrustName = "Test Trust",
            AdvisoryBoardDate = DateTime.Today,
            AdvisoryBoardConditions = "Some Conditions",
            SchoolSharePointLink = "https://school.sharepoint.com",
            IncomingTrustSharePointLink = "https://incoming.sharepoint.com",
            IsHandingToRCS = false,
            DirectiveAcademyOrder = true,
            IsDueTo2RI = true,
            HandoverComments = "Test Note"
        };

        var claims = new[] { new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "TestUserId") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        pageModel.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };


        // Act
        var result = await pageModel.OnPost(CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Contains($"/projects/conversion-projects/{projectId.Value}/created", redirectResult.Url);
    }

    [Fact]
    public async Task OnPost_WhenNotFoundExceptionThrown_LogsErrorAndReturnsPageResult()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        var mockErrorService = new Mock<ErrorService>();
        var mockLogger = new Mock<ILogger<CreateNewProject>>();

        var exceptionMessage = "Test not found exception";
        var notFoundException = new NotFoundException(exceptionMessage);
        mockSender
            .Setup(s => s.Send(It.IsAny<CreateMatConversionProjectCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(notFoundException);

        var pageModel = new CreateNewProject(mockSender.Object, mockErrorService.Object, mockLogger.Object)
        {
            URN = "123456",
            TrustReferenceNumber = "TR12345",
            TrustName = "Test Trust",
            AdvisoryBoardDate = DateTime.Today,
            SchoolSharePointLink = "https://school.sharepoint.com",
            IncomingTrustSharePointLink = "https://incoming.sharepoint.com",
            IsHandingToRCS = false,
            DirectiveAcademyOrder = true,
            IsDueTo2RI = true,
            HandoverComments = "Test Note"
        };

        var claims = new[] { new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "TestUserId") };
        pageModel.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext
                { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType")) }
        };

        // Act
        await pageModel.OnPost(CancellationToken.None);

        // Assert

        Assert.True(pageModel.ModelState.ContainsKey("NotFound"));
        Assert.Equal(exceptionMessage, pageModel.ModelState["NotFound"]?.Errors[0].ErrorMessage);

        mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(exceptionMessage)),
                It.Is<NotFoundException>(ex => ex.Message == exceptionMessage),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task OnPost_WhenGeneralExceptionThrown_LogsErrorAndReturnsPageResult()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        var mockErrorService = new Mock<ErrorService>();
        var mockLogger = new Mock<ILogger<CreateNewProject>>();

        var exceptionMessage = "General error";
        var generalException = new Exception(exceptionMessage);
        mockSender
            .Setup(s => s.Send(It.IsAny<CreateMatConversionProjectCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(generalException);

        var pageModel = new CreateNewProject(mockSender.Object, mockErrorService.Object, mockLogger.Object)
        {
            URN = "123456",
            TrustReferenceNumber = "TR12345",
            TrustName = "Test Trust",
            AdvisoryBoardDate = DateTime.Today,
            SchoolSharePointLink = "https://school.sharepoint.com",
            IncomingTrustSharePointLink = "https://incoming.sharepoint.com",
            IsHandingToRCS = false,
            DirectiveAcademyOrder = true,
            IsDueTo2RI = true,
            HandoverComments = "Test Note"
        };

        var claims = new[] { new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "TestUserId") };
        pageModel.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"))
            }
        };

        // Act
        await pageModel.OnPost(CancellationToken.None);

        // Assert
        Assert.True(pageModel.ModelState.ContainsKey("UnexpectedError"));
        Assert.Equal("An unexpected error occurred. Please try again later.",
            pageModel.ModelState["UnexpectedError"]?.Errors[0].ErrorMessage);

        mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("Error occurred while creating a conversion project.")),
                It.Is<Exception>(ex => ex.Message == exceptionMessage),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}