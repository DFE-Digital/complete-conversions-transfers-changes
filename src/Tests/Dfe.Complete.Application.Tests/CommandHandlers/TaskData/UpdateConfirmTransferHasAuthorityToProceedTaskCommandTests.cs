using AutoFixture.Xunit2;
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
    public class UpdateConfirmTransferHasAuthorityToProceedTaskCommandTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldThrowNotFoundExcepiton_IfProjectDoesNotExist(
            [Frozen] IProjectReadRepository projectReadRepository,
            UpdateConfirmTransferHasAuthorityToProceedTaskCommand command,
            UpdateConfirmTransferHasAuthorityToProceedTaskCommandHandler handler)
        {
            // Arrange    
            SetupProject(projectReadRepository);
           
            // Act  
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

            // Assert  
            Assert.Equal($"Project with task data {command.TaskDataId} not found.", exception.Message);
        }
        
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldThrowNotFoundExcepiton_IfTransferTaskDataDoesNotExist(
            [Frozen] IProjectReadRepository projectReadRepository,
            [Frozen] ITaskDataReadRepository taskDataReadRepository, 
            UpdateConfirmTransferHasAuthorityToProceedTaskCommandHandler handler)
        {
            // Arrange  

            var now = DateTime.UtcNow;
            var command = new UpdateConfirmTransferHasAuthorityToProceedTaskCommand(
                new TaskDataId(Guid.NewGuid()), false, false, false);
            SetupProject(projectReadRepository, command);
            var tasksData = new Domain.Entities.TransferTasksData(new TaskDataId(Guid.NewGuid()), now, now, false, false, false);
            taskDataReadRepository.TransferTaskData.Returns(new List<Domain.Entities.TransferTasksData> { tasksData }.AsQueryable().BuildMock());

            // Act  
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));

            // Assert  
            Assert.Equal($"Transfer task data {command.TaskDataId} not found.", exception.Message);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldUpdate_IfTransferTaskDataExists(
            [Frozen] IProjectReadRepository projectReadRepository,
            [Frozen] ITaskDataReadRepository taskDataReadRepository,
            UpdateConfirmTransferHasAuthorityToProceedTaskCommandHandler handler
        )
        {
            // Arrange
            var now = DateTime.UtcNow;
            var command = new UpdateConfirmTransferHasAuthorityToProceedTaskCommand(
                new TaskDataId(Guid.NewGuid()), false, false, false);

            var tasksData = new Domain.Entities.TransferTasksData(command.TaskDataId, now, now, true, true, true);
            taskDataReadRepository.TransferTaskData.Returns(new List<Domain.Entities.TransferTasksData> { tasksData }.AsQueryable().BuildMock());
            
            SetupProject(projectReadRepository, command);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        private static void SetupProject(IProjectReadRepository projectReadRepository, UpdateConfirmTransferHasAuthorityToProceedTaskCommand? command = null)
        {
            var now = DateTime.UtcNow;
            var project = new Domain.Entities.Project
            {
                Id = new ProjectId(Guid.NewGuid()),
                CreatedAt = now,
                TasksDataId = command?.TaskDataId
            };
            projectReadRepository.Projects.Returns(new List<Domain.Entities.Project> { project }.AsQueryable().BuildMock());
        }
    }
}
