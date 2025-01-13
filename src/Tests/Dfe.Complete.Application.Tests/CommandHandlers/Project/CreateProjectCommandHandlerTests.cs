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

        var now = DateTime.UtcNow;
        var project = CreateTestProject(user.Team, now);

        mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(project));

        // Act
        await handler.Handle(command, default);

        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(Arg.Is<Domain.Entities.Project>(s => s.Urn == command.Urn), default);
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
            .AddAsync(Arg.Is<Domain.Entities.Project>(s => s.Team == ProjectTeam.RegionalCaseWorkerServices.ToDescription()), default);
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
            .AddAsync(Arg.Is<Domain.Entities.Project>(p => p.Team == team && p.AssignedToId == user.Id && p.AssignedAt.HasValue), default);
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
            .AddAsync(Arg.Is<Domain.Entities.Project>(p => p.Team == team && p.AssignedToId == user.Id && p.AssignedAt.HasValue), default);
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
        command = command with { HandingOverToRegionalCaseworkService = false, HandoverComments = "this is a test note"};
        
    
        mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(project));
    
        // Act
        await handler.Handle(command, default);
    
        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(Arg.Is<Domain.Entities.Project>(p => p.Notes.FirstOrDefault().Body == command.HandoverComments), default);
    }
        
    private static Domain.Entities.Project CreateTestProject(string team, DateTime now) =>
        Domain.Entities.Project.CreateConversionProject(
            new ProjectId(Guid.NewGuid()),
            new Urn(2),
            now,
            now,
            TaskType.Conversion,
            ProjectType.Conversion.ToDescription(),
            Guid.NewGuid(),
            DateOnly.MinValue,
            true,
            new Ukprn(2),
            "region",
            true,
            true,
            DateOnly.MinValue,
            "",
            "",
            "",
            Guid.Empty,
            "",
            null,
            null,
            null
        );
}