using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using Moq;
using NSubstitute;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project
{
    public class UpdateHandoverWithDeliveryOfficerCommandTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnNotFoundException_IfConversionTaskDataDoesNotExist(
        [Frozen] IProjectReadRepository mockProjectRepository,
        [Frozen] ICompleteRepository<Domain.Entities.ConversionTasksData> mockConversionRepository,
        [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> mockTransferRepository,
        UpdateHandoverWithDeliveryOfficerCommand command
        )
        {
            // Arrange
            var now = DateTime.UtcNow;

            var sourceProject = Domain.Entities.Project.CreateConversionProject(command.ProjectId, new Urn(123456), now, now, TaskType.Conversion, ProjectType.Conversion, Guid.NewGuid(),
                DateOnly.MinValue, true, new Ukprn(2), Region.London, true, true, DateOnly.MinValue, "", "", "", null, default, new UserId(Guid.NewGuid()), null,
                null, null, Guid.NewGuid()); 

            mockProjectRepository.Projects.Returns(new List<Domain.Entities.Project> { sourceProject }.AsQueryable().BuildMock());

            var handler = new UpdateHandoverWithDeliveryOfficerCommandHandler(
                mockProjectRepository, mockConversionRepository, mockTransferRepository);

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

            // Assert
            Assert.Equal($"Conversion task data {sourceProject.TasksDataId} not found.", exception.Message); 

        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldThrowNotFoundExcepiton_IfTransferTaskDataDoesNotExist(
        [Frozen] IProjectReadRepository mockProjectRepository,
        [Frozen] ICompleteRepository<Domain.Entities.ConversionTasksData> mockConversionRepository,
        [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> mockTransferRepository,
        UpdateHandoverWithDeliveryOfficerCommand command)
        {
            // Arrange
            var now = DateTime.UtcNow;

            var sourceProject = Domain.Entities.Project.CreateConversionProject(command.ProjectId, new Urn(123456), now, now, TaskType.Transfer, ProjectType.Transfer, Guid.NewGuid(),
                DateOnly.MinValue, true, new Ukprn(2), Region.London, true, true, DateOnly.MinValue, "", "", "", null, default, new UserId(Guid.NewGuid()), null,
                null, null, Guid.NewGuid());

            mockProjectRepository.Projects.Returns(new List<Domain.Entities.Project> { sourceProject }.AsQueryable().BuildMock());

            var handler = new UpdateHandoverWithDeliveryOfficerCommandHandler(
                mockProjectRepository, mockConversionRepository, mockTransferRepository);

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

            // Assert
            Assert.Equal($"Transfer task data {sourceProject.TasksDataId} not found.", exception.Message);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldUpdate_IfConversionTaskDataExists(
           [Frozen] IProjectReadRepository mockProjectRepository,
           [Frozen] Mock<ICompleteRepository<Domain.Entities.ConversionTasksData>> mockConversionRepository,
           [Frozen] ICompleteRepository<Domain.Entities.TransferTasksData> mockTransferRepository,
           UpdateHandoverWithDeliveryOfficerCommand command
        )
        {
            // Arrange
            var now = DateTime.UtcNow;

            var sourceProject = Domain.Entities.Project.CreateConversionProject(command.ProjectId, new Urn(123456), now, now, TaskType.Conversion, ProjectType.Conversion, Guid.NewGuid(),
                DateOnly.MinValue, true, new Ukprn(2), Region.London, true, true, DateOnly.MinValue, "", "", "", null, default, new UserId(Guid.NewGuid()), null,
                null, null, Guid.NewGuid());
            var tasksData = new Domain.Entities.ConversionTasksData(sourceProject.TasksDataId!, now, now);
            mockConversionRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Domain.Entities.ConversionTasksData, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tasksData);
            mockProjectRepository.Projects.Returns(new List<Domain.Entities.Project> { sourceProject }.AsQueryable().BuildMock());

            var handler = new UpdateHandoverWithDeliveryOfficerCommandHandler(
                mockProjectRepository, mockConversionRepository.Object, mockTransferRepository);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

        }
    }
}
