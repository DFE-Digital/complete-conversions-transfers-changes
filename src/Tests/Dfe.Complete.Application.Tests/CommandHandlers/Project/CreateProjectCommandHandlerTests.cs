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

        var capturedNote = capturedProject.Notes.FirstOrDefault();
        Assert.Equal(command.HandoverComments, capturedNote?.Body);
        Assert.Equal("handover", capturedNote?.TaskIdentifier);
    }
    
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(ProjectCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldSetTeamToRcs_WhenHandoverToRcsTrue(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        CreateConversionProjectCommandHandler handler,
        CreateConversionProjectCommand command
    )
    {
        // Arrange
        command = command with { HandingOverToRegionalCaseworkService = true };

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

        // Assert
        Assert.NotNull(projectId);
        Assert.Equal(ProjectTeam.RegionalCaseWorkerServices, capturedProject.Team);
        Assert.Null(capturedProject.AssignedAt);
        Assert.Null(capturedProject.AssignedToId);
    }
    
     
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(ProjectCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldSetTeam_AssignedAt_AssignedTo_WhenNOTHandingOverToRcs(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        CreateConversionProjectCommandHandler handler,
        CreateConversionProjectCommand command
    )
    {
        // Arrange
        //NOT Handing over to Rcs
        command = command with { HandingOverToRegionalCaseworkService = false };

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

        // Assert
        Assert.NotNull(projectId);
        Assert.Equal(team, capturedProject.Team);
        Assert.NotNull(capturedProject.AssignedAt);
        Assert.NotNull(capturedProject.AssignedToId);
    }
}