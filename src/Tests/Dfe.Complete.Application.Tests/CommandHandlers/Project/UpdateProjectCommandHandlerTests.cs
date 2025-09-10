using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class UpdateAcademyUrnCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldUpdateAcademyUrn_WhenProjectExists(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        IFixture fixture)
    {
        // Arrange
        var existingProject = fixture.Create<Domain.Entities.Project>();
        var expectedUrn = new Urn(123456);
        var command = new UpdateAcademyUrnCommand(existingProject.Id, expectedUrn);

        var mockQueryable = new[] { existingProject }.AsQueryable().BuildMock();
        mockProjectRepository.Query().Returns(mockQueryable);

        var handler = new UpdateAcademyUrnCommandHandler(mockProjectRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedUrn, existingProject.AcademyUrn);
        await mockProjectRepository.Received(1).UpdateAsync(existingProject, Arg.Any<CancellationToken>());
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldNotThrowOrUpdate_WhenProjectNotFound(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository)
    {
        // Arrange
        var nonExistentProjectId = new ProjectId(Guid.NewGuid());
        var command = new UpdateAcademyUrnCommand(nonExistentProjectId, new Urn(987654));

        var emptyProjects = new List<Domain.Entities.Project>().AsQueryable().BuildMock();
        mockProjectRepository.Query().Returns(emptyProjects);

        var handler = new UpdateAcademyUrnCommandHandler(mockProjectRepository);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Null(exception);
        await mockProjectRepository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>());
    }
}
