using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Moq;
using NSubstitute;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project
{
    public class UpdateHandoverProjectCommandHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldUpdateProjectAndReturnSuccess_WhenProjectExists(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            UpdateHandoverProjectCommand command)
        {
            // Arrange
            var now = DateTime.UtcNow;
            var sourceProject = Domain.Entities.Project.CreateConversionProject(command.ProjectId, new Urn(123456), now, now, TaskType.Conversion, ProjectType.Conversion,
                Guid.NewGuid(), DateOnly.MinValue, true, new Ukprn(2), Region.London, true, true, DateOnly.MinValue, "", "", "", null, default, new UserId(Guid.NewGuid()),
                null, null, null, Guid.NewGuid());

            mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>(), It.IsAny<CancellationToken>()).Returns(sourceProject);

            var handler = new UpdateHandoverProjectCommandHandler(mockProjectRepository);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess); 
            await mockProjectRepository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Project>(p => p.Id == command.ProjectId), CancellationToken.None); 
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldThrowNotFoundException_WhenProjectDoesNotExist(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            UpdateHandoverProjectCommand command)
        {
            // Arrange
            mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>(), It.IsAny<CancellationToken>()).Returns((Domain.Entities.Project)null!);
            var handler = new UpdateHandoverProjectCommandHandler(mockProjectRepository);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }

}
