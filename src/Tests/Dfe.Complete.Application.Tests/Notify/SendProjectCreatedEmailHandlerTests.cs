using AutoFixture;
using Dfe.Complete.Application.Common.Constants;
using Dfe.Complete.Application.Common.EventHandlers;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using System.Linq;
using Xunit;

namespace Dfe.Complete.Application.Tests.Notify
{
    public class SendProjectCreatedEmailHandlerTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUserReadRepository> _userRepositoryMock;
        private readonly Mock<IProjectUrlBuilder> _projectUrlBuilderMock;
        private readonly Mock<ILogger<SendProjectCreatedEmailHandler>> _loggerMock;
        private readonly SendProjectCreatedEmailHandler _handler;
        private const string TestProjectBaseUrl = "https://test.complete.education.gov.uk/projects/";

        public SendProjectCreatedEmailHandlerTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _userRepositoryMock = new Mock<IUserReadRepository>();
            _projectUrlBuilderMock = new Mock<IProjectUrlBuilder>();
            _loggerMock = new Mock<ILogger<SendProjectCreatedEmailHandler>>();

            // Setup project URL builder to return test URLs
            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(It.IsAny<string>()))
                .Returns<string>(projectRef => $"{TestProjectBaseUrl}{projectRef}");

            _handler = new SendProjectCreatedEmailHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task HandleEvent_WhenProjectAssignedToRegionalTeam_SendsEmailsToTeamLeaders()
        {
            // Arrange
            var projectId = new ProjectId(Guid.NewGuid());
            var project = CreateTestProject(projectId, ProjectType.Conversion, ProjectTeam.RegionalCaseWorkerServices);
            var notification = new ProjectCreatedEvent(project);

            var teamLeaders = new List<User>
            {
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader1@education.gov.uk", FirstName = "Alice", ManageTeam = true, DeactivatedAt = null },
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader2@education.gov.uk", FirstName = "Bob", ManageTeam = true, DeactivatedAt = null }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult("msg-id", "ref", DateTime.UtcNow)));

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m => m.TemplateKey == EmailTemplateKeys.NewConversionProjectCreated),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task HandleEvent_WhenTransferProject_UsesCorrectTemplate()
        {
            // Arrange
            var projectId = new ProjectId(Guid.NewGuid());
            var project = CreateTestProject(projectId, ProjectType.Transfer, ProjectTeam.RegionalCaseWorkerServices);
            var notification = new ProjectCreatedEvent(project);

            var teamLeaders = new List<User>
            {
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader@education.gov.uk", FirstName = "Alice", ManageTeam = true, DeactivatedAt = null }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult("msg-id", "ref", DateTime.UtcNow)));

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m => m.TemplateKey == EmailTemplateKeys.NewTransferProjectCreated),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleEvent_WhenProjectNotAssignedToRegionalTeam_DoesNotSendEmails()
        {
            // Arrange
            var projectId = new ProjectId(Guid.NewGuid());
            var project = CreateTestProject(projectId, ProjectType.Conversion, ProjectTeam.London);
            var notification = new ProjectCreatedEvent(project);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleEvent_WhenNoActiveTeamLeaders_DoesNotSendEmails()
        {
            // Arrange
            var projectId = new ProjectId(Guid.NewGuid());
            var project = CreateTestProject(projectId, ProjectType.Conversion, ProjectTeam.RegionalCaseWorkerServices);
            var notification = new ProjectCreatedEvent(project);

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(new List<User>().AsQueryable().BuildMock());

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleEvent_WhenEmailSendFails_ContinuesSendingToOtherTeamLeaders()
        {
            // Arrange
            var projectId = new ProjectId(Guid.NewGuid());
            var project = CreateTestProject(projectId, ProjectType.Conversion, ProjectTeam.RegionalCaseWorkerServices);
            var notification = new ProjectCreatedEvent(project);

            var teamLeaders = new List<User>
            {
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader1@education.gov.uk", FirstName = "Alice", ManageTeam = true, DeactivatedAt = null },
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader2@education.gov.uk", FirstName = "Bob", ManageTeam = true, DeactivatedAt = null }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            // First call fails, second succeeds
            _emailSenderMock
                .SetupSequence(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Failure("Send failed", ErrorType.Unknown))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult("msg-id", "ref", DateTime.UtcNow)));

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task HandleEvent_IncludesCorrectPersonalisationFields()
        {
            // Arrange
            var projectId = new ProjectId(Guid.NewGuid());
            var project = CreateTestProject(projectId, ProjectType.Conversion, ProjectTeam.RegionalCaseWorkerServices);
            var notification = new ProjectCreatedEvent(project);

            var teamLeaders = new List<User>
            {
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader@education.gov.uk", FirstName = "Alice", ManageTeam = true, DeactivatedAt = null }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            EmailMessage? capturedMessage = null;
            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .Callback<EmailMessage, CancellationToken>((msg, ct) => capturedMessage = msg)
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult("msg-id", "ref", DateTime.UtcNow)));

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedMessage);
            Assert.Equal("Alice", capturedMessage!.Personalisation["first_name"]);
            Assert.Contains(projectId.Value.ToString(), capturedMessage.Personalisation["project_url"]);
            Assert.StartsWith(TestProjectBaseUrl, capturedMessage.Personalisation["project_url"]);
        }

        private static Project CreateTestProject(ProjectId projectId, ProjectType projectType, ProjectTeam team)
        {
            // Use AutoFixture to generate valid URN
            var fixture = new Fixture();
            var giasEstablishment = fixture.Customize(new GiasEstablishmentsCustomization()).Create<GiasEstablishment>();
            var urn = giasEstablishment.Urn ?? new Urn(12345); // Use valid URN from customization
            
            var now = DateTime.UtcNow;
            var userId = new UserId(Guid.NewGuid());
            var ukprn = new Ukprn(10001234);
            var region = Region.London;
            var tasksDataId = Guid.NewGuid();
            var localAuthorityId = Guid.NewGuid();

            if (projectType == ProjectType.Conversion)
            {
                var project = Project.CreateConversionProject(new CreateConversionProjectParams(
                    projectId,
                    urn,
                    tasksDataId,
                    DateOnly.FromDateTime(now.AddMonths(6)),
                    ukprn,
                    region,
                    false,
                    DateOnly.FromDateTime(now.AddMonths(3)),
                    "Test conditions",
                    null,
                    userId,
                    localAuthorityId));
                
                project.Team = team;
                if (team != ProjectTeam.RegionalCaseWorkerServices)
                {
                    project.AssignedToId = userId;
                    project.AssignedAt = now;
                }
                
                return project;
            }
            else
            {
                var project = Project.CreateTransferProject(new CreateTransferProjectParams(
                    projectId,
                    urn,
                    tasksDataId,
                    DateOnly.FromDateTime(now.AddMonths(6)),
                    ukprn,
                    new Ukprn(10005678),
                    region,
                    DateOnly.FromDateTime(now.AddMonths(3)),
                    "Test conditions",
                    null,
                    userId,
                    localAuthorityId));
                
                project.Team = team;
                if (team != ProjectTeam.RegionalCaseWorkerServices)
                {
                    project.AssignedToId = userId;
                    project.AssignedAt = now;
                }
                
                return project;
            }
        }
    }
}

