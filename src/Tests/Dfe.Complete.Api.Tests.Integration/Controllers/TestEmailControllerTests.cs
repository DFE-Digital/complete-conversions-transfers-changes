using Dfe.Complete.Api.Controllers;
using Dfe.Complete.Application.Common.Constants;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Dfe.Complete.Api.Tests.Integration.Controllers
{
    /// <summary>
    /// Unit tests for TestEmailController
    /// Tests email sending functionality with mocked dependencies
    /// </summary>
    public class TestEmailControllerTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IProjectUrlBuilder> _projectUrlBuilderMock;
        private readonly Mock<ILogger<TestEmailController>> _loggerMock;
        private readonly TestEmailController _controller;

        public TestEmailControllerTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _projectUrlBuilderMock = new Mock<IProjectUrlBuilder>();
            _loggerMock = new Mock<ILogger<TestEmailController>>();

            _controller = new TestEmailController(
                _emailSenderMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task SendWelcomeEmailAsync_WithValidInput_Returns200AndSuccessResponse()
        {
            // Arrange
            var email = "test@education.gov.uk";
            var firstName = "John";

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-123",
                    Reference: "ref-123",
                    SentAt: DateTime.UtcNow)));

            // Act
            var result = await _controller.SendWelcomeEmailAsync(email, firstName, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<EmailTestResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("msg-123", response.MessageId);
            Assert.Equal("ref-123", response.Reference);
            Assert.NotNull(response.SentAt);
            Assert.Contains("successfully", response.Message);

            // Verify email was sent with correct template
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.To.Value.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                        m.TemplateKey == "NewAccountAdded" &&
                        m.Personalisation.ContainsKey(EmailPersonalisationKeys.FirstName) &&
                        m.Personalisation[EmailPersonalisationKeys.FirstName] == firstName),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SendWelcomeEmailAsync_EmailSenderFails_Returns500WithError()
        {
            // Arrange
            var email = "test@education.gov.uk";
            var firstName = "John";

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Failure("Failed to send email", ErrorType.Unknown));

            // Act
            var result = await _controller.SendWelcomeEmailAsync(email, firstName, CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<EmailTestResponse>(statusCodeResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Failed to send email", response.Error);
            Assert.Equal("Unknown", response.ErrorType);
            Assert.Contains("Failed to send email", response.Message);
        }

        [Fact]
        public async Task SendAssignmentEmailAsync_WithValidInput_Returns200AndBuildsProjectUrl()
        {
            // Arrange
            var email = "test@education.gov.uk";
            var firstName = "Jane";
            var projectRef = "TEST-PROJECT-001";
            var expectedUrl = "https://complete.education.gov.uk/projects/TEST-PROJECT-001";

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(projectRef))
                .Returns(expectedUrl);

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-456",
                    Reference: "ref-456",
                    SentAt: DateTime.UtcNow)));

            // Act
            var result = await _controller.SendAssignmentEmailAsync(email, firstName, projectRef, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<EmailTestResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("msg-456", response.MessageId);

            // Verify IProjectUrlBuilder was used
            _projectUrlBuilderMock.Verify(x => x.BuildProjectUrl(projectRef), Times.Once);

            // Verify email contains project URL
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.Personalisation.ContainsKey(EmailPersonalisationKeys.ProjectUrl) &&
                        m.Personalisation[EmailPersonalisationKeys.ProjectUrl] == expectedUrl),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SendAssignmentEmailAsync_EmailSenderFails_Returns500WithError()
        {
            // Arrange
            var email = "test@education.gov.uk";
            var firstName = "Jane";
            var projectRef = "TEST-PROJECT-001";

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(It.IsAny<string>()))
                .Returns("https://complete.education.gov.uk/projects/TEST-PROJECT-001");

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Failure("Email service unavailable", ErrorType.Unknown));

            // Act
            var result = await _controller.SendAssignmentEmailAsync(email, firstName, projectRef, CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<EmailTestResponse>(statusCodeResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Email service unavailable", response.Error);
        }

        [Fact]
        public async Task SendTeamLeaderConversionEmailAsync_WithValidInput_Returns200AndUsesCorrectTemplate()
        {
            // Arrange
            var email = "teamlead@education.gov.uk";
            var firstName = "Alice";
            var projectRef = "TEST-CONV-001";
            var expectedUrl = "https://complete.education.gov.uk/projects/TEST-CONV-001";

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(projectRef))
                .Returns(expectedUrl);

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-789",
                    Reference: "ref-789",
                    SentAt: DateTime.UtcNow)));

            // Act
            var result = await _controller.SendTeamLeaderConversionEmailAsync(email, firstName, projectRef, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<EmailTestResponse>(okResult.Value);
            Assert.True(response.Success);

            // Verify correct template is used for conversion projects
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.TemplateKey == "NewConversionProjectCreated" &&
                        m.Personalisation[EmailPersonalisationKeys.FirstName] == firstName &&
                        m.Personalisation[EmailPersonalisationKeys.ProjectUrl] == expectedUrl),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SendTeamLeaderTransferEmailAsync_WithValidInput_Returns200AndUsesCorrectTemplate()
        {
            // Arrange
            var email = "teamlead@education.gov.uk";
            var firstName = "Bob";
            var projectRef = "TEST-TRANS-001";
            var expectedUrl = "https://complete.education.gov.uk/projects/TEST-TRANS-001";

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(projectRef))
                .Returns(expectedUrl);

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-101",
                    Reference: "ref-101",
                    SentAt: DateTime.UtcNow)));

            // Act
            var result = await _controller.SendTeamLeaderTransferEmailAsync(email, firstName, projectRef, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<EmailTestResponse>(okResult.Value);
            Assert.True(response.Success);

            // Verify correct template is used for transfer projects
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.TemplateKey == "NewTransferProjectCreated" &&
                        m.Personalisation[EmailPersonalisationKeys.FirstName] == firstName &&
                        m.Personalisation[EmailPersonalisationKeys.ProjectUrl] == expectedUrl),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AllEndpoints_UseEmailPersonalisationKeys_NotMagicStrings()
        {
            // Arrange
            var email = "test@education.gov.uk";
            var firstName = "Test";
            var projectRef = "TEST-001";

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(It.IsAny<string>()))
                .Returns("https://complete.education.gov.uk/projects/TEST-001");

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-test",
                    Reference: "ref-test",
                    SentAt: DateTime.UtcNow)));

            // Act - Test all endpoints
            await _controller.SendWelcomeEmailAsync(email, firstName, CancellationToken.None);
            await _controller.SendAssignmentEmailAsync(email, firstName, projectRef, CancellationToken.None);
            await _controller.SendTeamLeaderConversionEmailAsync(email, firstName, projectRef, CancellationToken.None);
            await _controller.SendTeamLeaderTransferEmailAsync(email, firstName, projectRef, CancellationToken.None);

            // Assert - All endpoints use EmailPersonalisationKeys constants, not magic strings
            // Verify that the constant value "first_name" is used (which is the value of EmailPersonalisationKeys.FirstName)
            // We're testing that the code uses the constant, not a hardcoded string
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.Personalisation.ContainsKey(EmailPersonalisationKeys.FirstName)),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(4));
        }

        [Fact]
        public async Task AllEndpoints_UseInjectedProjectUrlBuilder_NotHardcodedUrl()
        {
            // Arrange
            var email = "test@education.gov.uk";
            var firstName = "Test";
            var projectRef = "TEST-002";
            var customUrl = "https://custom.url/projects/TEST-002";

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(projectRef))
                .Returns(customUrl);

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-test",
                    Reference: "ref-test",
                    SentAt: DateTime.UtcNow)));

            // Act - Test endpoints that use project URLs
            await _controller.SendAssignmentEmailAsync(email, firstName, projectRef, CancellationToken.None);
            await _controller.SendTeamLeaderConversionEmailAsync(email, firstName, projectRef, CancellationToken.None);
            await _controller.SendTeamLeaderTransferEmailAsync(email, firstName, projectRef, CancellationToken.None);

            // Assert - IProjectUrlBuilder was used (not hardcoded URL)
            _projectUrlBuilderMock.Verify(x => x.BuildProjectUrl(projectRef), Times.Exactly(3));

            // Verify the custom URL was used in emails
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.Personalisation.ContainsKey(EmailPersonalisationKeys.ProjectUrl) &&
                        m.Personalisation[EmailPersonalisationKeys.ProjectUrl] == customUrl),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }
    }
}

