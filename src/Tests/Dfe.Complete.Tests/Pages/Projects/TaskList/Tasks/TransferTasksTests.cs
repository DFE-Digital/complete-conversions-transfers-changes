using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.TaskList;
using FluentAssertions;

namespace Dfe.Complete.Tests.Pages.Projects.TaskList.Tasks
{
    public class TransferTasksTests
    {
        [Fact]
        public void GetTasks_WithValidInputs_ReturnsAllTaskLists()
        {
            var transferTaskList = new TransferTaskListViewModel();
            var projectId = "project-123";

            var result = TransferTasks.BuildTaskList(transferTaskList, projectId);

            result.ProjectKickoffTasks.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
            result.ReadyToTransferTasks.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
            result.LegalDocumentsTasks.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
            result.AfterTransferTasks.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
        }
    }
}
