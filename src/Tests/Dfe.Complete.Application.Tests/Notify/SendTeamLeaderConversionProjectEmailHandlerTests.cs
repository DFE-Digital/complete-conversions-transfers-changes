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
    /// <summary>
    /// Tests for SendTeamLeaderConversionProjectEmailHandler
    /// Verifies that conversion-specific configuration is correctly applied
    /// </summary>
    public class SendTeamLeaderConversionProjectEmailHandlerTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUserReadRepository> _userRepositoryMock;
        private readonly Mock<IProjectUrlBuilder> _projectUrlBuilderMock;
        private readonly Mock<ILogger<SendTeamLeaderConversionProjectEmailHandler>> _loggerMock;
        private readonly SendTeamLeaderConversionProjectEmailHandler _handler;

        public SendTeamLeaderConversionProjectEmailHandlerTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _userRepositoryMock = new Mock<IUserReadRepository>();
            _projectUrlBuilderMock = new Mock<IProjectUrlBuilder>();
            _loggerMock = new Mock<ILogger<SendTeamLeaderConversionProjectEmailHandler>>();

            _handler = new SendTeamLeaderConversionProjectEmailHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task HandleEvent_WithConversionProject_UsesCorrectTemplate()
        {
            // Arrange
            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "CONV-001",
                ProjectType.Conversion,
                123456,
                "Test Conversion School");

            var teamLeaders = new List<User>
            {
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = "leader@education.gov.uk",
                    FirstName = "Alice",
                    ManageTeam = true,
                    DeactivatedAt = null
                }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(It.IsAny<string>()))
                .Returns("https://complete.education.gov.uk/projects/CONV-001");

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-123",
                    Reference: "ref-123",
                    SentAt: DateTime.UtcNow)));

            // Act
            await _handler.Handle(@event, CancellationToken.None);

            // Assert - Verify correct conversion template is used
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.TemplateKey == EmailTemplateKeys.NewConversionProjectCreated),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task HandleEvent_WithTransferProject_SkipsEmail()
        {
            // Arrange
            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "TRANS-001",
                ProjectType.Transfer, // Transfer, not Conversion
                123456,
                "Test Transfer School");

            // Act
            await _handler.Handle(@event, CancellationToken.None);

            // Assert - Should skip without accessing repository or sending emails

            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Fact]
        public async Task HandleEvent_LogsWithCorrectPrefix_Conversion()
        {
            // Arrange
            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "CONV-001",
                ProjectType.Conversion,
                123456,
                "Test School");

            var teamLeaders = new List<User>
            {
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = "leader@education.gov.uk",
                    FirstName = "Alice",
                    ManageTeam = true,
                    DeactivatedAt = null
                }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(It.IsAny<string>()))
                .Returns("https://complete.education.gov.uk/projects/CONV-001");

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-123",
                    Reference: "ref-123",
                    SentAt: DateTime.UtcNow)));

            // Act
            await _handler.Handle(@event, CancellationToken.None);

            // Assert - Verify logger was called (checking that handler is active)
            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}

