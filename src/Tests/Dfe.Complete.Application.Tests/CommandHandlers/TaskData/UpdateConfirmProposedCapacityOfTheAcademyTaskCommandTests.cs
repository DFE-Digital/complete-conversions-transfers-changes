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
    public class UpdateConfirmProposedCapacityOfTheAcademyTaskCommandTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldThrowNotFoundExcepiton_IfConversionTaskDataDoesNotExist(
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            [Frozen] ITaskDataWriteRepository taskDataWriteRepository,
            UpdateConfirmProposedCapacityOfTheAcademyTaskCommand command)
        {
            // Arrange  
            var now = DateTime.UtcNow;
            var tasksData = new Domain.Entities.ConversionTasksData(new TaskDataId(Guid.NewGuid()), now, now);
            taskDataReadRepository.ConversionTaskData.Returns(new List<Domain.Entities.ConversionTasksData> { tasksData }.AsQueryable().BuildMock());

            var handler = new UpdateConfirmProposedCapacityOfTheAcademyTaskCommandHandler(
                 taskDataReadRepository, taskDataWriteRepository);

            // Act  
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

            // Assert  
            Assert.Equal($"Conversion task data {command.TaskDataId} not found.", exception.Message);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldUpdate_IfConversionTaskDataExists(
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            [Frozen] ITaskDataWriteRepository taskDataWriteRepository,
            UpdateConfirmProposedCapacityOfTheAcademyTaskCommand command
        )
        {
            // Arrange
            var now = DateTime.UtcNow;
            var tasksData = new Domain.Entities.ConversionTasksData(command.TaskDataId, now, now);
            taskDataReadRepository.ConversionTaskData.Returns(new List<Domain.Entities.ConversionTasksData> { tasksData }.AsQueryable().BuildMock());

            var handler = new UpdateConfirmProposedCapacityOfTheAcademyTaskCommandHandler(taskDataReadRepository, taskDataWriteRepository);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }
    }
}
