using System.Linq.Expressions;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class CreateConversionProjectCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(ProjectCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        CreateConversionProjectCommandHandler handler,
        CreateConversionProjectCommand command
    )
    {
        // Arrange
        const ProjectTeam team = ProjectTeam.WestMidlands;
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team.ToDescription()
        };

        var createdAt = DateTime.UtcNow;
        var conversionTaskId = Guid.NewGuid();
        var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), createdAt, createdAt);

        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(Task.FromResult(user));

        var groupId = new ProjectGroupId(Guid.NewGuid());
        mockProjectGroupRepository.FindAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ProjectGroup { Id = groupId }));
        
        Domain.Entities.Project capturedProject = null!;
        
        mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));

        mockProjectRepository.AddAsync(capturedProject, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));
        
        mockConversionTaskRepository.AddAsync(Arg.Any<ConversionTasksData>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(conversionTask));

        // Act
        var projectId = await handler.Handle(command, default);

        Assert.NotNull(projectId);
        Assert.IsType<ProjectId>(projectId);

        await mockProjectRepository.Received(1).AddAsync(capturedProject);
        await mockConversionTaskRepository.Received(1).AddAsync(Arg.Any<ConversionTasksData>());

        Assert.Equal(command.Urn, capturedProject.Urn);
        Assert.Equal(command.SignificantDate, capturedProject.SignificantDate);
        Assert.Equal(command.IsSignificantDateProvisional, capturedProject.SignificantDateProvisional);
        Assert.Equal(command.IncomingTrustUkprn, capturedProject.IncomingTrustUkprn);
        Assert.Equal(command.IsDueTo2Ri, capturedProject.TwoRequiresImprovement);
        Assert.Equal(command.HasAcademyOrderBeenIssued, capturedProject.DirectiveAcademyOrder);
        Assert.Equal(command.EstablishmentSharepointLink, capturedProject.EstablishmentSharepointLink);
        Assert.Equal(command.IncomingTrustSharepointLink, capturedProject.IncomingTrustSharepointLink);
        Assert.Equal(groupId.Value, capturedProject.GroupId);
        Assert.Equal(command.HandoverComments, capturedProject.Notes.FirstOrDefault()?.Body);
    }

    //
    // [Theory]
    // [CustomAutoData(typeof(DateOnlyCustomization))]
    // public async Task Handle_ShouldSetTeamToRCcs_WhenHandOverToRcsIsTrue(
    //     [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
    //     CreateConversionProjectCommandHandler handler,
    //     CreateConversionProjectCommand command
    // )
    // {
    //     // Arrange
    //     var team = ProjectTeam.WestMidlands;
    //     var user = new User
    //     {
    //         Id = new UserId(Guid.NewGuid()),
    //         Team = team.ToDescription()
    //     };
    //     mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
    //
    //     var now = DateTime.UtcNow;
    //     var project = CreateTestProject(team, now);
    //     command = command with { HandingOverToRegionalCaseworkService = true };
    //
    //     mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
    //         .Returns(Task.FromResult(project));
    //
    //     // Act
    //     await handler.Handle(command, default);
    //
    //     // Assert
    //     await mockProjectRepository.Received(1)
    //         .AddAsync(
    //             Arg.Is<Domain.Entities.Project>(s => s.Team == ProjectTeam.RegionalCaseWorkerServices),
    //             default);
    // }
    //
    // [Theory]
    // [CustomAutoData(typeof(DateOnlyCustomization))]
    // public async Task Handle_ShouldSet_Team_And_AssignedTo_And_AssignedAt_WhenHandingOverToRcsFalse(
    //     [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
    //     CreateConversionProjectCommandHandler handler,
    //     CreateConversionProjectCommand command
    // )
    // {
    //     // Arrange
    //     var team = ProjectTeam.WestMidlands;
    //     var user = new User
    //     {
    //         Id = new UserId(Guid.NewGuid()),
    //         Team = team.ToDescription()
    //     };
    //
    //     mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
    //
    //     var now = DateTime.UtcNow;
    //     var project = CreateTestProject(team, now);
    //     command = command with { HandingOverToRegionalCaseworkService = false };
    //
    //     mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
    //         .Returns(Task.FromResult(project));
    //
    //     // Act
    //     await handler.Handle(command, default);
    //
    //     // Assert
    //     await mockProjectRepository.Received(1)
    //         .AddAsync(
    //             Arg.Is<Domain.Entities.Project>(p =>
    //                 p.Team == team
    //                 && p.AssignedToId == user.Id
    //                 && p.AssignedAt.HasValue), default);
    // }
    //
    // [Theory]
    // [CustomAutoData(typeof(DateOnlyCustomization))]
    // public async Task Handle_ShouldSetHandover(
    //     [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
    //     CreateConversionProjectCommandHandler handler,
    //     CreateConversionProjectCommand command
    // )
    // {
    //     // Arrange
    //     var team = ProjectTeam.WestMidlands;
    //     var user = new User
    //     {
    //         Id = new UserId(Guid.NewGuid()),
    //         Team = team.ToDescription()
    //     };
    //
    //     mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
    //
    //     var now = DateTime.UtcNow;
    //     var project = CreateTestProject(team, now);
    //     command = command with { HandingOverToRegionalCaseworkService = false };
    //
    //     mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
    //         .Returns(Task.FromResult(project));
    //
    //     // Act
    //     await handler.Handle(command, default);
    //
    //     // Assert
    //     await mockProjectRepository.Received(1)
    //         .AddAsync(
    //             Arg.Is<Domain.Entities.Project>(p =>
    //                 p.Team == team
    //                 && p.AssignedToId == user.Id
    //                 && p.AssignedAt.HasValue), default);
    // }
    //
    // [Theory]
    // [CustomAutoData(typeof(DateOnlyCustomization))]
    // public async Task Handle_ShouldAddNotes_WhenHandoverComments(
    //     [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
    //     CreateConversionProjectCommandHandler handler,
    //     CreateConversionProjectCommand command
    // )
    // {
    //     // Arrange
    //     var team = ProjectTeam.WestMidlands;
    //     var user = new User
    //     {
    //         Id = new UserId(Guid.NewGuid()),
    //         Team = team.ToDescription()
    //     };
    //     mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
    //
    //     var now = DateTime.UtcNow;
    //     var project = CreateTestProject(team, now);
    //     command = command with
    //     {
    //         HandingOverToRegionalCaseworkService = false, HandoverComments = "this is a test note"
    //     };
    //
    //
    //     mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
    //         .Returns(Task.FromResult(project));
    //
    //     // Act
    //     await handler.Handle(command, default);
    //
    //     // Assert
    //     await mockProjectRepository.Received(1)
    //         .AddAsync(Arg.Is<Domain.Entities.Project>(p => p.Notes.FirstOrDefault().Body == command.HandoverComments),
    //             default);
    // }

    // private static Domain.Entities.Project CreateTestProject(ProjectTeam team, DateTime now)
    // {
    //     var projectId = new ProjectId(Guid.NewGuid());
    //     var urn = new Urn(123456);
    //     var tasksDataId = Guid.NewGuid();
    //     var region = Region.NorthWest;
    //     var rdoId = new UserId(Guid.NewGuid());
    //     var assignedTo = new UserId(Guid.NewGuid());
    //     var assignedAt = now;
    //
    //     var project = Domain.Entities.Project.CreateConversionProject(
    //         Id: projectId,
    //         urn: urn,
    //         createdAt: now,
    //         updatedAt: now,
    //         taskType: TaskType.Conversion,
    //         projectType: ProjectType.Conversion,
    //         tasksDataId: tasksDataId,
    //         significantDate: new DateOnly(2025, 01, 01),
    //         isSignificantDateProvisional: true,
    //         incomingTrustUkprn: new Ukprn(99999999),
    //         region: region,
    //         isDueTo2RI: true,
    //         hasAcademyOrderBeenIssued: true,
    //         advisoryBoardDate: new DateOnly(2025, 06, 15),
    //         advisoryBoardConditions: "Some conditions",
    //         establishmentSharepointLink: "EstablishmentLink",
    //         incomingTrustSharepointLink: "IncomingTrustLink",
    //         groupId: Guid.NewGuid(),
    //         team: team,
    //         regionalDeliveryOfficerId: rdoId,
    //         assignedToId: assignedTo,
    //         assignedAt: assignedAt
    //     );
    //
    //     return project;
    // }
}