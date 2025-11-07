using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils.Exceptions;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class UpdateAssignedUserCommandHandlerTests
{
    private readonly ICompleteRepository<Domain.Entities.Project> _mockProjectRepository;
    private readonly ICompleteRepository<User> _mockUserRepository;
    private readonly UpdateAssignedUser _handler;

    public UpdateAssignedUserCommandHandlerTests()
    {
        _mockProjectRepository = Substitute.For<ICompleteRepository<Domain.Entities.Project>>();
        _mockUserRepository = Substitute.For<ICompleteRepository<User>>();
        _handler = new UpdateAssignedUser(_mockProjectRepository, _mockUserRepository);
    }
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_UpdatesTheUserId(UpdateAssignedUserCommand command)
    {
        // Arrange
        var parameters = new CreateConversionProjectParams(
            command.ProjectId,
            new Urn(123456),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            new Ukprn(2),
            Region.London,
            true,
            DateOnly.FromDateTime(DateTime.Today.AddDays(15)),
            "Advisory board conditions",
            null,
            new UserId(Guid.NewGuid()),
            Guid.NewGuid()
        );

        var sourceProject = Domain.Entities.Project.CreateConversionProject(parameters);

        _mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>()).Returns(sourceProject);
        _mockUserRepository.GetAsync(Arg.Is<UserId>(id => id.Value.ToString() == command.AssignedUser.Value.ToString()), Arg.Any<CancellationToken>()).Returns(new User { Id = command.AssignedUser, AssignToProject = true });

        // Act & Assert
        await _handler.Handle(command, CancellationToken.None);
        await _mockProjectRepository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Project>(p => p.AssignedToId == command.AssignedUser), CancellationToken.None);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ThrowsIfTheUserIsNotAssignable(UpdateAssignedUserCommand command)
    {
        // Arrange
        var parameters = new CreateConversionProjectParams(
            command.ProjectId,
            new Urn(123456),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            new Ukprn(2),
            Region.London,
            true,
            DateOnly.FromDateTime(DateTime.Today.AddDays(15)),
            "Advisory board conditions",
            null,
            new UserId(Guid.NewGuid()),
            Guid.NewGuid()
        );

        var sourceProject = Domain.Entities.Project.CreateConversionProject(parameters);

        _mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>()).Returns(sourceProject);
        _mockUserRepository.GetAsync(Arg.Is<UserId>(id => id.Value.ToString() == command.AssignedUser.Value.ToString()), Arg.Any<CancellationToken>()).Returns(new User { Id = command.AssignedUser, AssignToProject = false });

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));
        await _mockProjectRepository.Received(0).UpdateAsync(Arg.Is<Domain.Entities.Project>(p => p.RegionalDeliveryOfficerId == command.AssignedUser), CancellationToken.None);
    }
}