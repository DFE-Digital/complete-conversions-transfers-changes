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

public class UpdateRegionalDeliveryOfficerCommandHandlerTests
{
    private readonly ICompleteRepository<Domain.Entities.Project> _mockProjectRepository;
    private readonly ICompleteRepository<User> _mockUserRepository;
    private readonly UpdateRegionalDeliveryOfficer _handler;

    public UpdateRegionalDeliveryOfficerCommandHandlerTests()
    {
        _mockProjectRepository = Substitute.For<ICompleteRepository<Domain.Entities.Project>>();
        _mockUserRepository = Substitute.For<ICompleteRepository<User>>();
        _handler = new UpdateRegionalDeliveryOfficer(_mockProjectRepository, _mockUserRepository);
    }
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_UpdatesTheUserId(UpdateRegionalDeliveryOfficerCommand command)
    {
        // Arrange
        var sourceParameters = new CreateConversionProjectParams(
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

        var updatedParameters = new CreateConversionProjectParams(
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
            command.RegionalDeliveryOfficer,
            Guid.NewGuid()
        );

        var sourceProject = Domain.Entities.Project.CreateConversionProject(sourceParameters);
        var updatedProject = Domain.Entities.Project.CreateConversionProject(updatedParameters);

        _mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(sourceProject);
        _mockProjectRepository
            .UpdateAsync(
                Arg.Is<Domain.Entities.Project>(p => p.RegionalDeliveryOfficerId == command.RegionalDeliveryOfficer),
                Arg.Any<CancellationToken>()).Returns(updatedProject);
        _mockUserRepository
            .GetAsync(Arg.Is<UserId>(id => id.Value.ToString() == command.RegionalDeliveryOfficer.Value.ToString()),
                Arg.Any<CancellationToken>()).Returns(new User
                { Id = command.RegionalDeliveryOfficer, AssignToProject = true });

        // Act & Assert
        await _handler.Handle(command, CancellationToken.None);
        await _mockProjectRepository.Received(1)
            .UpdateAsync(
                Arg.Is<Domain.Entities.Project>(p => p.RegionalDeliveryOfficerId == command.RegionalDeliveryOfficer),
                CancellationToken.None);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ThrowsIfTheUserIsNotAssignable(UpdateRegionalDeliveryOfficerCommand command)
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

        _mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>())
            .Returns(sourceProject);
        _mockUserRepository
            .GetAsync(Arg.Is<UserId>(id => id.Value.ToString() == command.RegionalDeliveryOfficer.Value.ToString()),
                Arg.Any<CancellationToken>()).Returns(new User
                { Id = command.RegionalDeliveryOfficer, AssignToProject = false });

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));
        await _mockProjectRepository.Received(0)
            .UpdateAsync(
                Arg.Is<Domain.Entities.Project>(p => p.RegionalDeliveryOfficerId == command.RegionalDeliveryOfficer),
                CancellationToken.None);
    }
}