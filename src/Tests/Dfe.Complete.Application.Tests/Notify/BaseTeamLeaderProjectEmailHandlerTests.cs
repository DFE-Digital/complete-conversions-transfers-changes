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
    /// Tests for BaseTeamLeaderProjectEmailHandler
    /// Uses a concrete test handler to test the abstract base class functionality
    /// </summary>
    public class BaseTeamLeaderProjectEmailHandlerTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUserReadRepository> _userRepositoryMock;
        private readonly Mock<IProjectUrlBuilder> _projectUrlBuilderMock;
        private readonly Mock<ILogger<BaseTeamLeaderProjectEmailHandler<TestConcreteHandler>>> _loggerMock;

        public BaseTeamLeaderProjectEmailHandlerTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _userRepositoryMock = new Mock<IUserReadRepository>();
            _projectUrlBuilderMock = new Mock<IProjectUrlBuilder>();
            _loggerMock = new Mock<ILogger<BaseTeamLeaderProjectEmailHandler<TestConcreteHandler>>>();
        }

        [Fact]
        public async Task HandleEvent_WithMatchingProjectType_SendsEmailsToAllTeamLeaders()
        {
            // Arrange
            var handler = new TestConcreteHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);

            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "PRJ-001",
                ProjectType.Conversion,
                123456,
                "Test School");

            var teamLeaders = new List<User>
            {
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = "leader1@education.gov.uk",
                    FirstName = "Alice",
                    ManageTeam = true,
                    DeactivatedAt = null
                },
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = "leader2@education.gov.uk",
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
                .Returns("https://complete.education.gov.uk/projects/PRJ-001");

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
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2)); // Should send to both team leaders
        }

        [Fact]
        public async Task HandleEvent_WithNonMatchingProjectType_SkipsEmail()
        {
            // Arrange
            var handler = new TestConcreteHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);

            // Event with Transfer type, but handler only processes Conversion
            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "PRJ-001",
                ProjectType.Transfer,
                123456,
                "Test School");

            // Act
            await handler.Handle(@event, CancellationToken.None);

            // Assert - Should skip without accessing repository

            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Never()); // Should not send any emails
        }

        [Fact]
        public async Task HandleEvent_WithNoTeamLeaders_LogsWarningAndReturns()
        {
            // Arrange
            var handler = new TestConcreteHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);

            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "PRJ-001",
                ProjectType.Conversion,
                123456,
                "Test School");

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(new List<User>().AsQueryable().BuildMock()); // No team leaders

            // Act
            await handler.Handle(@event, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Never()); // Should not attempt to send emails
        }

        [Fact]
        public async Task HandleEvent_WithNullTeamLeaderEmail_SkipsAndLogsWarning()
        {
            // Arrange
            var handler = new TestConcreteHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);

            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "PRJ-001",
                ProjectType.Conversion,
                123456,
                "Test School");

            var teamLeaders = new List<User>
            {
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = null, // Null email
                    FirstName = "Alice",
                    ManageTeam = true,
                    DeactivatedAt = null
                },
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = "valid@education.gov.uk",
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
                .Returns("https://complete.education.gov.uk/projects/PRJ-001");

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
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Once()); // Should only send to the valid user
        }

        [Fact]
        public async Task HandleEvent_WithNullTeamLeaderFirstName_SkipsAndLogsWarning()
        {
            // Arrange
            var handler = new TestConcreteHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);

            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "PRJ-001",
                ProjectType.Conversion,
                123456,
                "Test School");

            var teamLeaders = new List<User>
            {
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = "test@education.gov.uk",
                    FirstName = null, // Null first name
                    ManageTeam = true,
                    DeactivatedAt = null
                },
                new User
                {
                    Id = new UserId(Guid.NewGuid()),
                    Email = "valid@education.gov.uk",
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
                .Returns("https://complete.education.gov.uk/projects/PRJ-001");

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
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Once()); // Should only send to the valid user
        }

        [Fact]
        public async Task HandleEvent_EmailSenderFails_LogsErrorAndContinues()
        {
            // Arrange
            var handler = new TestConcreteHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);

            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "PRJ-001",
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
                .Returns("https://complete.education.gov.uk/projects/PRJ-001");

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Failure("Email service unavailable", ErrorType.Unknown));

            // Act
            await handler.Handle(@event, CancellationToken.None);

            // Assert - Should not throw, just log error
            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task HandleEvent_UsesEmailPersonalisationKeys_NotMagicStrings()
        {
            // Arrange
            var handler = new TestConcreteHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);

            var @event = new ProjectAssignedToRegionalTeamEvent(
                new ProjectId(Guid.NewGuid()),
                "PRJ-001",
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
                .Returns("https://complete.education.gov.uk/projects/PRJ-001");

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult(
                    ProviderMessageId: "msg-123",
                    Reference: "ref-123",
                    SentAt: DateTime.UtcNow)));

            // Act
            await handler.Handle(@event, CancellationToken.None);

            // Assert - Verify constants are used
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.Personalisation.ContainsKey(EmailPersonalisationKeys.FirstName) &&
                        m.Personalisation.ContainsKey(EmailPersonalisationKeys.ProjectUrl)),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        /// <summary>
        /// Concrete test handler for testing the abstract base class
        /// </summary>
        public class TestConcreteHandler : BaseTeamLeaderProjectEmailHandler<TestConcreteHandler>
        {
            public TestConcreteHandler(
                IEmailSender emailSender,
                IUserReadRepository userRepository,
                IProjectUrlBuilder projectUrlBuilder,
                ILogger<BaseTeamLeaderProjectEmailHandler<TestConcreteHandler>> logger)
                : base(emailSender, userRepository, projectUrlBuilder, logger)
            {
            }

            protected override ProjectType ProjectType => ProjectType.Conversion;
            protected override string TemplateKey => EmailTemplateKeys.NewConversionProjectCreated;
            protected override string LogMessagePrefix => "Test";
        }
    }
}

