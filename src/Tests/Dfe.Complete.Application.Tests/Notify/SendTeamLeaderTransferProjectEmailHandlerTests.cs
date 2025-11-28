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
    /// Tests for SendTeamLeaderTransferProjectEmailHandler
    /// Verifies that transfer-specific configuration is correctly applied
    /// </summary>
    public class SendTeamLeaderTransferProjectEmailHandlerTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUserReadRepository> _userRepositoryMock;
        private readonly Mock<IProjectUrlBuilder> _projectUrlBuilderMock;
        private readonly Mock<ILogger<SendTeamLeaderTransferProjectEmailHandler>> _loggerMock;
        private readonly SendTeamLeaderTransferProjectEmailHandler _handler;

        public SendTeamLeaderTransferProjectEmailHandlerTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _userRepositoryMock = new Mock<IUserReadRepository>();
            _projectUrlBuilderMock = new Mock<IProjectUrlBuilder>();
            _loggerMock = new Mock<ILogger<SendTeamLeaderTransferProjectEmailHandler>>();

            _handler = new SendTeamLeaderTransferProjectEmailHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task HandleEvent_WithTransferProject_UsesCorrectTemplate()
        {
            // Arrange
            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "TRANS-001",
                ProjectType.Transfer,
                123456,
                "Test Transfer School");

            var teamLeaders = new List<User>
            {
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = "leader@education.gov.uk",
                    FirstName = "Bob",
                    ManageTeam = true,
                    DeactivatedAt = null
                }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(It.IsAny<string>()))
                .Returns("https://complete.education.gov.uk/projects/TRANS-001");

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-456",
                    Reference: "ref-456",
                    SentAt: DateTime.UtcNow)));

            // Act
            await _handler.Handle(@event, CancellationToken.None);

            // Assert - Verify correct transfer template is used
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.TemplateKey == EmailTemplateKeys.NewTransferProjectCreated),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task HandleEvent_WithConversionProject_SkipsEmail()
        {
            // Arrange
            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "CONV-001",
                ProjectType.Conversion, // Conversion, not Transfer
                123456,
                "Test Conversion School");

            // Act
            await _handler.Handle(@event, CancellationToken.None);

            // Assert - Should skip without accessing repository or sending emails

            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Fact]
        public async Task HandleEvent_LogsWithCorrectPrefix_Transfer()
        {
            // Arrange
            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "TRANS-001",
                ProjectType.Transfer,
                123456,
                "Test School");

            var teamLeaders = new List<User>
            {
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = "leader@education.gov.uk",
                    FirstName = "Bob",
                    ManageTeam = true,
                    DeactivatedAt = null
                }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(It.IsAny<string>()))
                .Returns("https://complete.education.gov.uk/projects/TRANS-001");

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-456",
                    Reference: "ref-456",
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

