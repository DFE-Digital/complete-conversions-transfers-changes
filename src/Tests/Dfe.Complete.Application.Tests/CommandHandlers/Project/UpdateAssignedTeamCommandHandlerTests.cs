using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class UpdateAssignedTeamCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_UpdatesTheTeam(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        UpdateAssignedTeamCommand command
    )
    {
        // Arrange
        var now = DateTime.UtcNow;

        var sourceProject = new Domain.Entities.Project()
        {
            Id = command.ProjectId,
            Urn = new Urn(123456),
            CreatedAt = now,
            UpdatedAt = now,
            TasksDataType = TaskType.Conversion,
            Type = ProjectType.Conversion,
            TasksDataId = new TaskDataId(Guid.NewGuid()),
            SignificantDate = DateOnly.MinValue,
            SignificantDateProvisional = true,
            IncomingTrustUkprn = new Ukprn(2),
            Region = Region.London,
            TwoRequiresImprovement = true,
            DirectiveAcademyOrder = true,
            AdvisoryBoardDate = DateOnly.MinValue,
            AdvisoryBoardConditions = "",
            EstablishmentSharepointLink = "",
            IncomingTrustSharepointLink = "",
            GroupId = null,
            RegionalDeliveryOfficerId = new UserId(Guid.NewGuid()),
            AssignedToId = null,
            AssignedAt = null,
            LocalAuthorityId = new LocalAuthorityId(Guid.NewGuid())
        };

        mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>()).Returns(sourceProject);

        var handler = new UpdateAssignedTeam(
            mockProjectRepository);

        // Act & Assert
        await handler.Handle(command, CancellationToken.None);
        await mockProjectRepository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Project>(p => p.Team == command.AssignedTeam), CancellationToken.None);
    }
}