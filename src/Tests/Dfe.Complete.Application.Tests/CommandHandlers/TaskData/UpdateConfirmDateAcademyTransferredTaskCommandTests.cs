using AutoFixture.Xunit2;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.CommandHandlers.TaskData
{
    public class UpdateConfirmDateAcademyTransferredTaskCommandTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldThrowNotFoundException_IfTransferTaskDataDoesNotExist(
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            [Frozen] ITaskDataWriteRepository taskDataWriteRepository,
            UpdateConfirmDateAcademyTransferredTaskCommand command)
        {
            // Arrange  
            var now = DateTime.UtcNow;
            var tasksData = new Domain.Entities.TransferTasksData(new TaskDataId(Guid.NewGuid()), now, now, false, false, false);
            taskDataReadRepository.TransferTaskData.Returns(new List<Domain.Entities.TransferTasksData> { tasksData }.AsQueryable().BuildMock());

            var handler = new UpdateConfirmDateAcademyTransferredTaskCommandHandler(taskDataReadRepository, taskDataWriteRepository);

            // Act  
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

            // Assert  
            Assert.Equal($"Transfer task data {command.TaskDataId} not found.", exception.Message);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldUpdateTransferredDate_IfTransferTaskDataExists(
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            [Frozen] ITaskDataWriteRepository taskDataWriteRepository,
            UpdateConfirmDateAcademyTransferredTaskCommand command)
        {
            // Arrange
            var now = DateTime.UtcNow;
            var tasksData = new Domain.Entities.TransferTasksData(command.TaskDataId, now, now, false, false, false);
            taskDataReadRepository.TransferTaskData.Returns(new List<Domain.Entities.TransferTasksData> { tasksData }.AsQueryable().BuildMock());

            var handler = new UpdateConfirmDateAcademyTransferredTaskCommandHandler(taskDataReadRepository, taskDataWriteRepository);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            Assert.Equal(command.DateAcademyTransferred, tasksData.ConfirmDateAcademyTransferredDateTransferred);
            await taskDataWriteRepository.Received(1).UpdateTransferAsync(tasksData, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldUpdateWithNullDate_IfDateAcademyTransferredIsNull(
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            [Frozen] ITaskDataWriteRepository taskDataWriteRepository,
            TaskDataId taskDataId)
        {
            // Arrange
            var now = DateTime.UtcNow;
            var command = new UpdateConfirmDateAcademyTransferredTaskCommand(taskDataId, null);
            var tasksData = new Domain.Entities.TransferTasksData(taskDataId, now, now, false, false, false);
            taskDataReadRepository.TransferTaskData.Returns(new List<Domain.Entities.TransferTasksData> { tasksData }.AsQueryable().BuildMock());

            var handler = new UpdateConfirmDateAcademyTransferredTaskCommandHandler(taskDataReadRepository, taskDataWriteRepository);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            Assert.Null(tasksData.ConfirmDateAcademyTransferredDateTransferred);
            await taskDataWriteRepository.Received(1).UpdateTransferAsync(tasksData, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        }
    }
}
