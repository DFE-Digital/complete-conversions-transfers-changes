using AutoFixture.Xunit2;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.CommandHandlers.TaskData
{
    public class UpdateDeedOfVariationTaskCommandTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnNotFoundException_IfConversionTaskDataDoesNotExist(
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            [Frozen] ITaskDataWriteRepository taskDataWriteRepository,
            UpdateDeedOfVariationTaskCommand command
        )
        {
            // Arrange
            var now = DateTime.UtcNow;
            command = command with { ProjectType = ProjectType.Conversion };
            var tasksData = new Domain.Entities.ConversionTasksData(new TaskDataId(Guid.NewGuid()), now, now);
            taskDataReadRepository.ConversionTaskData.Returns(new List<Domain.Entities.ConversionTasksData> { tasksData }.AsQueryable().BuildMock());

            var handler = new UpdateDeedOfVariationTaskCommandHandler(taskDataReadRepository, taskDataWriteRepository);

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

            // Assert
            Assert.Equal($"Conversion task data {command.TaskDataId} not found.", exception.Message);

        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldThrowNotFoundExcepiton_IfTransferTaskDataDoesNotExist(
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            [Frozen] ITaskDataWriteRepository taskDataWriteRepository,
            UpdateDeedOfVariationTaskCommand command)
        {
            // Arrange  
            command = command with { ProjectType = ProjectType.Transfer };
            var now = DateTime.UtcNow;
            var tasksData = new Domain.Entities.TransferTasksData(new TaskDataId(Guid.NewGuid()), now, now, false, false, false);
            taskDataReadRepository.TransferTaskData.Returns(new List<Domain.Entities.TransferTasksData> { tasksData }.AsQueryable().BuildMock());

            var handler = new UpdateDeedOfVariationTaskCommandHandler(
                 taskDataReadRepository, taskDataWriteRepository);

            // Act  
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

            // Assert  
            Assert.Equal($"Transfer task data {command.TaskDataId} not found.", exception.Message);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldUpdate_IfConversionTaskDataExists(
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            [Frozen] ITaskDataWriteRepository taskDataWriteRepository,
            UpdateDeedOfVariationTaskCommand command
        )
        {
            // Arrange
            command = command with { ProjectType = ProjectType.Conversion };
            var now = DateTime.UtcNow;
            var tasksData = new Domain.Entities.ConversionTasksData(command.TaskDataId, now, now);
            taskDataReadRepository.ConversionTaskData.Returns(new List<Domain.Entities.ConversionTasksData> { tasksData }.AsQueryable().BuildMock());

            var handler = new UpdateDeedOfVariationTaskCommandHandler(taskDataReadRepository, taskDataWriteRepository);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldUpdate_IfTransferTaskDataExists(
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            [Frozen] ITaskDataWriteRepository taskDataWriteRepository,
            UpdateDeedOfVariationTaskCommand command
        )
        {
            // Arrange
            command = command with { ProjectType = ProjectType.Transfer };
            var now = DateTime.UtcNow;
            var tasksData = new Domain.Entities.TransferTasksData(command.TaskDataId, now, now, false, false, false);
            taskDataReadRepository.TransferTaskData.Returns(new List<Domain.Entities.TransferTasksData> { tasksData }.AsQueryable().BuildMock());

            var handler = new UpdateDeedOfVariationTaskCommandHandler(taskDataReadRepository, taskDataWriteRepository);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }
    }
}
