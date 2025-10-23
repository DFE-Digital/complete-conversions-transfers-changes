using AutoFixture.Xunit2;
using Dfe.Complete.Application.KeyContacts.Interfaces;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils.Exceptions;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Microsoft.Extensions.Logging;
using MockQueryable;
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
            [Frozen] Projects.Interfaces.IKeyContactReadRepository mockKeyContactReadRepository,
            [Frozen] IKeyContactWriteRepository mockKeyContactWriteRepository,
            [Frozen] ILogger<UpdateHandoverProjectCommandHandler> mockLogger,
            KeyContact keyContact,
            UpdateHandoverProjectCommand command)
        {
            // Arrange
            var now = DateTime.UtcNow;
            var sourceProject = Domain.Entities.Project.CreateConversionProject(command.ProjectId, new Urn(123456), now, now, TaskType.Conversion, ProjectType.Conversion,
                Guid.NewGuid(), DateOnly.MinValue, true, new Ukprn(2), Region.London, true, true, DateOnly.MinValue, "", "", "", null, default, new UserId(Guid.NewGuid()),
                null, null, null, Guid.NewGuid());

            mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>(), It.IsAny<CancellationToken>()).Returns(sourceProject);
            var mockQueryable = new[] { keyContact }.AsQueryable().BuildMock();
            mockKeyContactReadRepository.KeyContacts.Returns(mockQueryable);
            var handler = new UpdateHandoverProjectCommandHandler(mockProjectRepository, mockKeyContactReadRepository, mockKeyContactWriteRepository, mockLogger);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            await mockProjectRepository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Project>(p => p.Id == command.ProjectId), CancellationToken.None);
            await mockKeyContactWriteRepository.Received(1).AddKeyContactAsync(Arg.Any<KeyContact>(), CancellationToken.None);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldLogErrorAndReturnSuccess_WhenKeyContactExists(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] Projects.Interfaces.IKeyContactReadRepository mockKeyContactReadRepository,
            [Frozen] IKeyContactWriteRepository mockKeyContactWriteRepository,
            [Frozen] ILogger<UpdateHandoverProjectCommandHandler> mockLogger,
            UpdateHandoverProjectCommand command,
            KeyContact keyContact)
        {
            // Arrange
            var now = DateTime.UtcNow;
            var sourceProject = Domain.Entities.Project.CreateConversionProject(command.ProjectId, new Urn(123456), now, now, TaskType.Conversion, ProjectType.Conversion,
                Guid.NewGuid(), DateOnly.MinValue, true, new Ukprn(2), Region.London, true, true, DateOnly.MinValue, "", "", "", null, default, new UserId(Guid.NewGuid()),
                null, null, null, Guid.NewGuid());

            mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>(), It.IsAny<CancellationToken>()).Returns(sourceProject);
            keyContact.ProjectId = sourceProject.Id;
            var queryableKeyContacts = new[] { keyContact }.AsQueryable().BuildMock();
            mockKeyContactReadRepository.KeyContacts.Returns(queryableKeyContacts);

            var handler = new UpdateHandoverProjectCommandHandler(mockProjectRepository, mockKeyContactReadRepository, mockKeyContactWriteRepository, mockLogger);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            mockLogger.Received().Log(LogLevel.Error, Arg.Any<EventId>(),
                Arg.Is<object>(v => v != null &&
                   v.ToString()!.Contains("Key contact already exists for handover project") &&
                   v.ToString()!.Contains(sourceProject.Id.Value.ToString())
                ), Arg.Any<Exception>(), Arg.Any<Func<object, Exception?, string>>());
            await mockKeyContactWriteRepository.Received(0).AddKeyContactAsync(Arg.Any<KeyContact>(), CancellationToken.None);
            await mockProjectRepository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Project>(p => p.Id == command.ProjectId), CancellationToken.None);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldThrowNotFoundException_WhenProjectDoesNotExist(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            [Frozen] Projects.Interfaces.IKeyContactReadRepository mockKeyContactReadRepository,
            [Frozen] IKeyContactWriteRepository mockKeyContactWriteRepository,
            [Frozen] ILogger<UpdateHandoverProjectCommandHandler> mockLogger,
            UpdateHandoverProjectCommand command)
        {
            // Arrange
            mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>(), It.IsAny<CancellationToken>()).Returns((Domain.Entities.Project)null!);
            var handler = new UpdateHandoverProjectCommandHandler(mockProjectRepository, mockKeyContactReadRepository, mockKeyContactWriteRepository, mockLogger);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }

}
