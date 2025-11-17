using Dfe.Complete.Application.Common.Constants;
using Dfe.Complete.Application.Common.EventHandlers;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using System.Linq;
using Xunit;

namespace Dfe.Complete.Application.Tests.Notify
{
    public class EmailEventHandlersTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUserReadRepository> _userRepositoryMock;

        public EmailEventHandlersTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _userRepositoryMock = new Mock<IUserReadRepository>();
        }

        [Fact]
        public async Task SendNewAccountEmailHandler_WithValidEvent_SendsEmail()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SendNewAccountEmailHandler>>();
            var handler = new SendNewAccountEmailHandler(_emailSenderMock.Object, loggerMock.Object);

            var @event = new UserCreatedEvent(
                new UserId(Guid.NewGuid()),
                "test@education.gov.uk",
                "John",
                "Doe",
                "Test Team");

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-123",
                    Reference: "ref-123",
                    SentAt: DateTime.UtcNow)));

            // Act
            await handler.Handle(@event, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.To.Value == "test@education.gov.uk" &&
                        m.TemplateKey == EmailTemplateKeys.NewAccountAdded &&
                        m.Personalisation.ContainsKey("first_name") &&
                        m.Personalisation["first_name"] == "John"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(ProjectType.Conversion, "AssignedNotificationConversion")]
        [InlineData(ProjectType.Transfer, "AssignedNotificationTransfer")]
        public async Task SendAssignedNotificationEmailHandler_WithActiveUser_SendsEmailWithCorrectTemplate(ProjectType projectType, string expectedTemplateKey)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SendAssignedNotificationEmailHandler>>();
            var projectUrlBuilderMock = new Mock<IProjectUrlBuilder>();
            projectUrlBuilderMock.Setup(x => x.BuildProjectUrl(It.IsAny<string>())).Returns("https://test.com/projects/test");
            var handler = new SendAssignedNotificationEmailHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                projectUrlBuilderMock.Object,
                loggerMock.Object);

            var userId = new UserId(Guid.NewGuid());
            var @event = new ProjectAssignedToUserEvent(
                new ProjectId(Guid.NewGuid()),
                "PRJ-001",
                projectType,
                userId,
                "test@education.gov.uk",
                "John",
                123456,
                "Test School");

            var user = new User
            {
                Id = userId,
                Email = "test@education.gov.uk",
                FirstName = "John",
                DeactivatedAt = null // IsActive is computed from DeactivatedAt
            };
            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(new List<User> { user }.AsQueryable().BuildMock());

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-123",
                    Reference: "ref-123",
                    SentAt: DateTime.UtcNow)));

            // Act
            await handler.Handle(@event, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.To.Value == "test@education.gov.uk" &&
                        m.TemplateKey == expectedTemplateKey &&
                        m.Personalisation.ContainsKey("first_name") &&
                        m.Personalisation["first_name"] == "John" &&
                        m.Personalisation.ContainsKey("project_url")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SendAssignedNotificationEmailHandler_WithNullUser_SkipsEmail()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SendAssignedNotificationEmailHandler>>();
            var projectUrlBuilderMock = new Mock<IProjectUrlBuilder>();
            projectUrlBuilderMock.Setup(x => x.BuildProjectUrl(It.IsAny<string>())).Returns("https://test.com/projects/test");
            var handler = new SendAssignedNotificationEmailHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                projectUrlBuilderMock.Object,
                loggerMock.Object);

            var userId = new UserId(Guid.NewGuid());
            var @event = new ProjectAssignedToUserEvent(
                new ProjectId(Guid.NewGuid()),
                "PRJ-001",
                ProjectType.Conversion,
                userId,
                "test@education.gov.uk",
                "John",
                123456,
                "Test School");

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(new List<User>().AsQueryable().BuildMock());

            // Act
            await handler.Handle(@event, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}

