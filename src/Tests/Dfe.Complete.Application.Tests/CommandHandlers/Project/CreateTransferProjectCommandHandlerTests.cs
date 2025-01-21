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

public class CreateTransferProjectCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        CreateTransferProjectCommandHandler handler,
        CreateTransferProjectCommand command
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
        var project = CreateTestTransferProject(user.Team, now);

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
        var project = CreateTestTransferProject(team, now);
        command = command with { HandingOverToRegionalCaseworkService = false, HandoverComments = "this is a test note"};
        
    
        mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(project));
    
        // Act
        await handler.Handle(command, default);
    
        // Assert
        await mockProjectRepository.Received(1)
            .AddAsync(Arg.Is<Domain.Entities.Project>(p => p.Notes.FirstOrDefault().Body == command.HandoverComments), default);
    }

    private static Domain.Entities.Project CreateTestTransferProject(string team, DateTime now) =>
        Domain.Entities.Project.CreateTransferProject(
            new ProjectId(Guid.NewGuid()),
            new Urn(2),
            now,
            now,
            TaskType.Transfer,
            ProjectType.Transfer,
            Guid.NewGuid(),
            "region",
            team,
            null,
            null,
            now,
            new Ukprn(1),
            new Ukprn(3),
            Guid.Empty,
            "",
            "",
            "",
            DateOnly.MinValue,
            "",
            DateOnly.MinValue,
            true,
            true
        );
}