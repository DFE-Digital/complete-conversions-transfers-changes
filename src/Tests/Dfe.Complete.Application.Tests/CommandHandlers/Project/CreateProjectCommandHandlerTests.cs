using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class CreateConversionProjectCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        CreateConversionProjectCommandHandler handler,
        CreateConversionProjectCommand command
    )
    {
        // Arrange
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Team = ProjectTeam.WestMidlands.ToDescription()
        };
        mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        var groupId = new ProjectGroupId(Guid.NewGuid());
        mockProjectRepository.GetProjectGroupIdByIdentifierAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(groupId);

        var now = DateTime.UtcNow;
        var project = CreateTestProject(user.Team, now);

        mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(project));
        
        // Act
        var projectId = await handler.Handle(command, default);
        
        Assert.NotNull(projectId);
        Assert.IsType<ProjectId>(projectId);
        
        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(Arg.Is<Domain.Entities.Project>(p =>
                p.Urn == command.Urn 
                && p.SignificantDate == command.SignificantDate 
                && p.SignificantDateProvisional == command.IsSignificantDateProvisional
                && p.IncomingTrustUkprn == command.IncomingTrustUkprn
                && p.TwoRequiresImprovement == command.IsDueTo2Ri
                && p.DirectiveAcademyOrder == command.HasAcademyOrderBeenIssued
                && p.EstablishmentSharepointLink == command.EstablishmentSharepointLink 
                && p.IncomingTrustSharepointLink == command.IncomingTrustSharepointLink
                && p.GroupId == groupId.Value
                && p.Notes.FirstOrDefault().Body == command.HandoverComments), default);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldSetTeamToRCcs_WhenHandOverToRcsIsTrue(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        CreateConversionProjectCommandHandler handler,
        CreateConversionProjectCommand command
    )
    {
        // Arrange
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Team = ProjectTeam.WestMidlands.ToDescription()
        };
        mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        var now = DateTime.UtcNow;
        var project = CreateTestProject(user.Team, now);
        command = command with { HandingOverToRegionalCaseworkService = true };

        mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(project));

        // Act
        await handler.Handle(command, default);

        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(
                Arg.Is<Domain.Entities.Project>(s => s.Team == ProjectTeam.RegionalCaseWorkerServices.ToDescription()),
                default);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldSet_Team_And_AssignedTo_And_AssignedAt_WhenHandingOverToRcsFalse(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        CreateConversionProjectCommandHandler handler,
        CreateConversionProjectCommand command
    )
    {
        // Arrange
        var team = ProjectTeam.WestMidlands.ToDescription();
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team
        };

        mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        var now = DateTime.UtcNow;
        var project = CreateTestProject(team, now);
        command = command with { HandingOverToRegionalCaseworkService = false };

        mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(project));

        // Act
        await handler.Handle(command, default);

        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(
                Arg.Is<Domain.Entities.Project>(p =>
                    p.Team == team && p.AssignedToId == user.Id && p.AssignedAt.HasValue), default);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldSetHandover(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        CreateConversionProjectCommandHandler handler,
        CreateConversionProjectCommand command
    )
    {
        // Arrange
        var team = ProjectTeam.WestMidlands.ToDescription();
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team
        };

        mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        var now = DateTime.UtcNow;
        var project = CreateTestProject(team, now);
        command = command with { HandingOverToRegionalCaseworkService = false };

        mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(project));

        // Act
        await handler.Handle(command, default);

        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(
                Arg.Is<Domain.Entities.Project>(p =>
                    p.Team == team && p.AssignedToId == user.Id && p.AssignedAt.HasValue), default);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldAddNotes_WhenHandoverComments(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        CreateConversionProjectCommandHandler handler,
        CreateConversionProjectCommand command
    )
    {
        // Arrange
        var team = ProjectTeam.WestMidlands.ToDescription();
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team
        };
        mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        var now = DateTime.UtcNow;
        var project = CreateTestProject(team, now);
        command = command with
        {
            HandingOverToRegionalCaseworkService = false, HandoverComments = "this is a test note"
        };


        mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(project));

        // Act
        await handler.Handle(command, default);

        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(Arg.Is<Domain.Entities.Project>(p => p.Notes.FirstOrDefault().Body == command.HandoverComments),
                default);
    }

    private static Domain.Entities.Project CreateTestProject(string team, DateTime now)
    {
        var projectId = new ProjectId(Guid.NewGuid());
        var urn = new Urn(123456);
        var tasksDataId = Guid.NewGuid();
        var region = "West Midlands";
        var rdoId = new UserId(Guid.NewGuid()); 
        var assignedTo = new UserId(Guid.NewGuid()); 
        var assignedAt = now; 

        var project = Domain.Entities.Project.CreateConversionProject(
            Id: projectId,
            urn: urn,
            createdAt: now,
            updatedAt: now,
            taskType: TaskType.Conversion,
            projectType: ProjectType.Conversion,
            tasksDataId: tasksDataId,
            significantDate: new DateOnly(2025, 01, 01),
            isSignificantDateProvisional: true,
            incomingTrustUkprn: new Ukprn(99999999),
            region: region,
            isDueTo2RI: true,
            hasAcademyOrderBeenIssued: true,
            advisoryBoardDate: new DateOnly(2025, 06, 15),
            advisoryBoardConditions: "Some conditions",
            establishmentSharepointLink: "EstablishmentLink",
            incomingTrustSharepointLink: "IncomingTrustLink",
            groupId: Guid.NewGuid(),
            team: team,
            regionalDeliveryOfficerId: rdoId,
            assignedToId: assignedTo,
            assignedAt: assignedAt
        );

        return project;
    }
}