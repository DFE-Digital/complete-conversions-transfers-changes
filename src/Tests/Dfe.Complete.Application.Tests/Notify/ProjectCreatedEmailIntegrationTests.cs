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
    /// <summary>
    /// Integration tests for the complete flow of email notifications on project creation.
    /// Tests the interaction between domain events, event handlers, and email sending.
    /// </summary>
    public class ProjectCreatedEmailIntegrationTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUserReadRepository> _userRepositoryMock;
        private readonly Mock<IProjectUrlBuilder> _projectUrlBuilderMock;
        private readonly SendProjectCreatedEmailHandler _handler;
        private const string TestProjectBaseUrl = "https://test.complete.education.gov.uk/projects/";

        public ProjectCreatedEmailIntegrationTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _userRepositoryMock = new Mock<IUserReadRepository>();
            _projectUrlBuilderMock = new Mock<IProjectUrlBuilder>();

            // Setup project URL builder to return test URLs
            _projectUrlBuilderMock
                .Setup(x => x.BuildProjectUrl(It.IsAny<string>()))
                .Returns<string>(projectRef => $"{TestProjectBaseUrl}{projectRef}");

            var loggerMock = new Mock<ILogger<SendProjectCreatedEmailHandler>>();

            _handler = new SendProjectCreatedEmailHandler(
                _emailSenderMock.Object,
                _userRepositoryMock.Object,
                _projectUrlBuilderMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task EndToEnd_ConversionProjectCreatedAndAssignedToRegionalTeam_SendsEmailsToAllTeamLeaders()
        {
            // Arrange: Create a conversion project assigned to regional team
            var fixture = new Fixture();
            var giasEstablishment = fixture.Customize(new GiasEstablishmentsCustomization()).Create<GiasEstablishment>();
            var urn = giasEstablishment.Urn ?? new Urn(123456); // Use valid URN from customization
            
            var projectId = new ProjectId(Guid.NewGuid());
            var ukprn = new Ukprn(10001234);
            var userId = new UserId(Guid.NewGuid());
            var now = DateTime.UtcNow;
            var tasksDataId = Guid.NewGuid();
            var localAuthorityId = Guid.NewGuid();

            var project = Project.CreateConversionProject(new CreateConversionProjectParams(
                projectId,
                urn,
                tasksDataId,
                DateOnly.FromDateTime(now.AddMonths(6)),
                ukprn,
                Region.London,
                false,
                DateOnly.FromDateTime(now.AddMonths(3)),
                "Advisory board conditions",
                null,
                userId,
                localAuthorityId));
            
            project.Team = ProjectTeam.RegionalCaseWorkerServices; // Assigned to regional team

            // Setup: 3 active team leaders
            var teamLeaders = new List<User>
            {
                new() { Id = new UserId(Guid.NewGuid()), Email = "team.leader1@education.gov.uk", FirstName = "Alice", ManageTeam = true, DeactivatedAt = null },
                new() { Id = new UserId(Guid.NewGuid()), Email = "team.leader2@education.gov.uk", FirstName = "Bob", ManageTeam = true, DeactivatedAt = null },
                new() { Id = new UserId(Guid.NewGuid()), Email = "team.leader3@education.gov.uk", FirstName = "Charlie", ManageTeam = true, DeactivatedAt = null }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(
                    new EmailSendResult("notify-message-id", "unique-reference", DateTime.UtcNow)));

            // Act: Raise the domain event as if the project was just created
            var projectCreatedEvent = new ProjectCreatedEvent(project);
            await _handler.Handle(projectCreatedEvent, CancellationToken.None);

            // Assert: Email sent to all 3 team leaders with correct template and personalisation
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.TemplateKey == EmailTemplateKeys.NewConversionProjectCreated &&
                        m.Personalisation.ContainsKey("first_name") &&
                        m.Personalisation.ContainsKey("project_url") &&
                        m.Personalisation["project_url"].Contains(projectId.Value.ToString())),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(3));

            // Verify each team leader received an email
            foreach (var leader in teamLeaders)
            {
                _emailSenderMock.Verify(
                    x => x.SendAsync(
                        It.Is<EmailMessage>(m =>
                            m.To.Value == leader.Email &&
                            m.Personalisation["first_name"] == leader.FirstName),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
            }
        }

        [Fact]
        public async Task EndToEnd_TransferProjectCreatedAndAssignedToRegionalTeam_SendsEmailsWithTransferTemplate()
        {
            // Arrange: Create a transfer project assigned to regional team
            var fixture = new Fixture();
            var giasEstablishment = fixture.Customize(new GiasEstablishmentsCustomization()).Create<GiasEstablishment>();
            var urn = giasEstablishment.Urn ?? new Urn(654321); // Use valid URN from customization
            
            var projectId = new ProjectId(Guid.NewGuid());
            var userId = new UserId(Guid.NewGuid());
            var now = DateTime.UtcNow;
            var tasksDataId = Guid.NewGuid();
            var localAuthorityId = Guid.NewGuid();

            var project = Project.CreateTransferProject(new CreateTransferProjectParams(
                projectId,
                urn,
                tasksDataId,
                DateOnly.FromDateTime(now.AddMonths(6)),
                new Ukprn(10001234),
                new Ukprn(10005678),
                Region.NorthWest,
                DateOnly.FromDateTime(now.AddMonths(3)),
                "Advisory board conditions",
                null,
                userId,
                localAuthorityId));
            
            project.Team = ProjectTeam.RegionalCaseWorkerServices; // Assigned to regional team

            // Setup: 2 active team leaders
            var teamLeaders = new List<User>
            {
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader1@education.gov.uk", FirstName = "Dave", ManageTeam = true, DeactivatedAt = null },
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader2@education.gov.uk", FirstName = "Eve", ManageTeam = true, DeactivatedAt = null }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            _emailSenderMock
                .Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Success(
                    new EmailSendResult("notify-message-id", "unique-reference", DateTime.UtcNow)));

            // Act: Raise the domain event
            var projectCreatedEvent = new ProjectCreatedEvent(project);
            await _handler.Handle(projectCreatedEvent, CancellationToken.None);

            // Assert: Email sent with transfer template
            _emailSenderMock.Verify(
                x => x.SendAsync(
                    It.Is<EmailMessage>(m =>
                        m.TemplateKey == EmailTemplateKeys.NewTransferProjectCreated),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task EndToEnd_ProjectCreatedAndAssignedToSpecificUser_DoesNotSendEmailsToTeamLeaders()
        {
            // Arrange: Create a conversion project assigned to a specific user (not regional team)
            var fixture = new Fixture();
            var giasEstablishment = fixture.Customize(new GiasEstablishmentsCustomization()).Create<GiasEstablishment>();
            var urn = giasEstablishment.Urn ?? new Urn(789012); // Use valid URN from customization
            
            var projectId = new ProjectId(Guid.NewGuid());
            var assignedUserId = new UserId(Guid.NewGuid());
            var now = DateTime.UtcNow;
            var tasksDataId = Guid.NewGuid();
            var localAuthorityId = Guid.NewGuid();

            var project = Project.CreateConversionProject(new CreateConversionProjectParams(
                projectId,
                urn,
                tasksDataId,
                DateOnly.FromDateTime(now.AddMonths(6)),
                new Ukprn(10001234),
                Region.SouthEast,
                false,
                DateOnly.FromDateTime(now.AddMonths(3)),
                "Advisory board conditions",
                null,
                assignedUserId,
                localAuthorityId));
            
            project.Team = ProjectTeam.London; // Assigned to a regional team, not RegionalCaseWorkerServices
            project.AssignedToId = assignedUserId; // Assigned to specific user
            project.AssignedAt = now;

            // Act: Raise the domain event
            var projectCreatedEvent = new ProjectCreatedEvent(project);
            await _handler.Handle(projectCreatedEvent, CancellationToken.None);

            // Assert: No emails sent (because project not assigned to RegionalCaseWorkerServices)
            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task EndToEnd_EmailSendingPartiallyFails_LogsErrorsButContinuesSending()
        {
            // Arrange: Create a conversion project
            var fixture = new Fixture();
            var giasEstablishment = fixture.Customize(new GiasEstablishmentsCustomization()).Create<GiasEstablishment>();
            var urn = giasEstablishment.Urn ?? new Urn(345678); // Use valid URN from customization
            
            var projectId = new ProjectId(Guid.NewGuid());
            var userId = new UserId(Guid.NewGuid());
            var now = DateTime.UtcNow;
            var tasksDataId = Guid.NewGuid();
            var localAuthorityId = Guid.NewGuid();

            var project = Project.CreateConversionProject(new CreateConversionProjectParams(
                projectId,
                urn,
                tasksDataId,
                DateOnly.FromDateTime(now.AddMonths(6)),
                new Ukprn(10001234),
                Region.EastMidlands,
                false,
                DateOnly.FromDateTime(now.AddMonths(3)),
                "Advisory board conditions",
                null,
                userId,
                localAuthorityId));
            
            project.Team = ProjectTeam.RegionalCaseWorkerServices;

            // Setup: 3 team leaders
            var teamLeaders = new List<User>
            {
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader1@education.gov.uk", FirstName = "Frank", ManageTeam = true, DeactivatedAt = null },
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader2@education.gov.uk", FirstName = "Grace", ManageTeam = true, DeactivatedAt = null },
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader3@education.gov.uk", FirstName = "Henry", ManageTeam = true, DeactivatedAt = null }
            };

            _userRepositoryMock
                .Setup(x => x.Users)
                .Returns(teamLeaders.AsQueryable().BuildMock());

            // Setup: First email fails, others succeed
            _emailSenderMock
                .SetupSequence(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmailSendResult>.Failure("Temporary service failure", ErrorType.Unknown))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult("msg-2", "ref-2", DateTime.UtcNow)))
                .ReturnsAsync(Result<EmailSendResult>.Success(new EmailSendResult("msg-3", "ref-3", DateTime.UtcNow)));

            // Act: Raise the domain event
            var projectCreatedEvent = new ProjectCreatedEvent(project);
            await _handler.Handle(projectCreatedEvent, CancellationToken.None);

            // Assert: All 3 attempts were made despite first failure
            _emailSenderMock.Verify(
                x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }

        [Fact]
        public async Task EndToEnd_ProjectUrlIncludesConfiguredBaseUrl()
        {
            // Arrange
            var fixture = new Fixture();
            var giasEstablishment = fixture.Customize(new GiasEstablishmentsCustomization()).Create<GiasEstablishment>();
            var urn = giasEstablishment.Urn ?? new Urn(111222); // Use valid URN from customization
            
            var projectId = new ProjectId(Guid.NewGuid());
            var userId = new UserId(Guid.NewGuid());
            var now = DateTime.UtcNow;
            var tasksDataId = Guid.NewGuid();
            var localAuthorityId = Guid.NewGuid();

            var project = Project.CreateConversionProject(new CreateConversionProjectParams(
                projectId,
                urn,
                tasksDataId,
                DateOnly.FromDateTime(now.AddMonths(6)),
                new Ukprn(10001234),
                Region.WestMidlands,
                false,
                DateOnly.FromDateTime(now.AddMonths(3)),
                "Advisory board conditions",
                null,
                userId,
                localAuthorityId));
            
            project.Team = ProjectTeam.RegionalCaseWorkerServices;

            var teamLeaders = new List<User>
            {
                new() { Id = new UserId(Guid.NewGuid()), Email = "leader@education.gov.uk", FirstName = "Ian", ManageTeam = true, DeactivatedAt = null }
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
            var projectCreatedEvent = new ProjectCreatedEvent(project);
            await _handler.Handle(projectCreatedEvent, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedMessage);
            var expectedUrl = $"{TestProjectBaseUrl}{projectId.Value}";
            Assert.Equal(expectedUrl, capturedMessage!.Personalisation["project_url"]);
        }
    }
}

