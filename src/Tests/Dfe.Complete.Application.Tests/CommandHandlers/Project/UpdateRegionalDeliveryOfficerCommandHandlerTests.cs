using System.Linq.Expressions;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class UpdateRegionalDeliveryOfficerCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_UpdatesTheUserId(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        UpdateRegionalDeliveryOfficerCommand command
    )
    {
        // Arrange
        var now = DateTime.UtcNow;

        var sourceProject = Domain.Entities.Project.CreateConversionProject(
            command.ProjectId,
            new Urn(123456),
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

        var updatedProject = Domain.Entities.Project.CreateConversionProject(
            command.ProjectId,
            new Urn(123456),
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
            command.RegionalDeliveryOfficer,
            null,
            null,
            null,
            Guid.NewGuid());

        mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(sourceProject);
        mockProjectRepository
            .UpdateAsync(
                Arg.Is<Domain.Entities.Project>(p => p.RegionalDeliveryOfficerId == command.RegionalDeliveryOfficer),
                Arg.Any<CancellationToken>()).Returns(updatedProject);
        mockUserRepository
            .GetAsync(Arg.Is<UserId>(id => id.Value.ToString() == command.RegionalDeliveryOfficer.Value.ToString()),
                Arg.Any<CancellationToken>()).Returns(new User
                { Id = command.RegionalDeliveryOfficer, AssignToProject = true });

        var handler = new UpdateRegionalDeliveryOfficer(
            mockProjectRepository, mockUserRepository);

        // Act & Assert
        await handler.Handle(command, CancellationToken.None);
        await mockProjectRepository.Received(1)
            .UpdateAsync(
                Arg.Is<Domain.Entities.Project>(p => p.RegionalDeliveryOfficerId == command.RegionalDeliveryOfficer),
                CancellationToken.None);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ThrowsIfTheUserIsNotAssignable(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        UpdateRegionalDeliveryOfficerCommand command
    )
    {
        // Arrange
        var now = DateTime.UtcNow;

        var sourceProject = Domain.Entities.Project.CreateConversionProject(
            command.ProjectId,
            new Urn(123456),
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

        mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>())
            .Returns(sourceProject);
        mockUserRepository
            .GetAsync(Arg.Is<UserId>(id => id.Value.ToString() == command.RegionalDeliveryOfficer.Value.ToString()),
                Arg.Any<CancellationToken>()).Returns(new User
                { Id = command.RegionalDeliveryOfficer, AssignToProject = false });

        var handler = new UpdateRegionalDeliveryOfficer(
            mockProjectRepository, mockUserRepository);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        await mockProjectRepository.Received(0)
            .UpdateAsync(
                Arg.Is<Domain.Entities.Project>(p => p.RegionalDeliveryOfficerId == command.RegionalDeliveryOfficer),
                CancellationToken.None);
    }
}