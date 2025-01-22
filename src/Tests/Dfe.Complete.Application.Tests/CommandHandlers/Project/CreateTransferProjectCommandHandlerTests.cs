using System.Linq.Expressions;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class CreateTransferProjectCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        CreateTransferProjectCommandHandler handler,
        CreateTransferProjectCommand command
    )
    {
        // Arrange
        const ProjectTeam team = ProjectTeam.WestMidlands;
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team.ToDescription()
        };
        // mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(Task.FromResult(user));
        
        var groupId = new ProjectGroupId(Guid.NewGuid());
        mockProjectGroupRepository.FindAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ProjectGroup { Id = groupId }));
        
        Domain.Entities.Project capturedProject = null!;
        
        mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));

        mockProjectRepository.AddAsync(capturedProject, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));
        
        // Act
        var projectId = await handler.Handle(command, default);

        Assert.NotNull(projectId);
        Assert.IsType<ProjectId>(projectId);

        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(capturedProject);
    }
    
    
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldAddNotes_WhenHandoverComments(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        CreateConversionProjectCommandHandler handler,
        CreateConversionProjectCommand command
    )
    {
        command = command with { HandingOverToRegionalCaseworkService = false, HandoverComments = "this is a test note"};
        
        const ProjectTeam team = ProjectTeam.WestMidlands;
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Team = team.ToDescription()
        };

        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(Task.FromResult(user));

        var groupId = new ProjectGroupId(Guid.NewGuid());
        mockProjectGroupRepository.FindAsync(Arg.Any<Expression<Func<ProjectGroup, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ProjectGroup { Id = groupId }));
        
        Domain.Entities.Project capturedProject = null!;
        
        mockProjectRepository.AddAsync(Arg.Do<Domain.Entities.Project>(proj => capturedProject = proj), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(capturedProject));
    
        // Act
        await handler.Handle(command, default);
    
        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(Arg.Is<Domain.Entities.Project>(p => p.Notes.FirstOrDefault().Body == command.HandoverComments), default);
    }
    
}