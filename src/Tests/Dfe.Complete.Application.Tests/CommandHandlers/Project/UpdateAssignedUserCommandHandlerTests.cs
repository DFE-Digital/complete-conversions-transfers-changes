using System.Linq.Expressions;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class UpdateAssignedUserCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_UpdatesTheUserId(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        UpdateAssignedUserCommand command
    )
    {
        // Arrange
        var now = DateTime.UtcNow;
        
        var sourceProject = Domain.Entities.Project.CreateConversionProject(
            new ProjectId(Guid.NewGuid()),
            command.ProjectUrn,
            now,
            now,
            Domain.Enums.TaskType.Conversion,
            Domain.Enums.ProjectType.Conversion,
            Guid.NewGuid(),
            DateOnly.MinValue,
            true,
            new Domain.ValueObjects.Ukprn(2),
            Region.London,
            true,
            true,
            DateOnly.MinValue,
            "",
            "",
            "",
            null,
            default,
            new UserId(Guid.NewGuid()),
            null,
            null,
            null,
            Guid.NewGuid());

        mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>()).Returns(sourceProject);

        var handler = new UpdateAssignedUser(
            mockProjectRepository);

        // Act & Assert
        await handler.Handle(command, CancellationToken.None);
        await mockProjectRepository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Project>(p => p.AssignedToId == command.AssignedUser), CancellationToken.None);
    }
}