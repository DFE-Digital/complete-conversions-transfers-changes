using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.TaskList.Tasks;
using FluentAssertions;

namespace Dfe.Complete.Tests.Pages.Projects.TaskList.Tasks
{
    public class ConversionTasksTests
    {
        [Fact]
        public void GetTasks_WithValidInputs_ReturnsAllTaskLists()
        {
            var conversionTaskList = new ConversionTaskListViewModel();
            var projectId = "project-123";

            var result = ConversionTasks.BuildTaskList(conversionTaskList, projectId);

            result.ProjectKickoffTasks.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
            result.LegalDocumentsTasks.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
            result.ReadyForOpeningTasks.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
            result.AfterOpeningTasks.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
        }

        [Fact]
        public void GetTasks_WithShowProcessConversionSupportGrant_ReturnsListWithItemAtCorrectPosition()
        {
            var conversionTaskList = new ConversionTaskListViewModel
            {
                ShowProcessConversionSupportGrant = true
            };
            var projectId = "project-123";
            var result = ConversionTasks.BuildTaskList(conversionTaskList, projectId).ProjectKickoffTasks;

            result.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
            result.Should().ContainSingle(x => x.Name == "Process conversion support grant");
            result.Should().OnlyHaveUniqueItems(x => x.DisplayOrder);
            result.First(x => x.Name == "Process conversion support grant").DisplayOrder.Should().Be(6);
        }

        [Fact]
        public void GetTasks_WithShowProcessConversionSupportGrantOfFalse_ShouldNotIncludeTask()
        {
            var conversionTaskList = new ConversionTaskListViewModel
            {
                ShowProcessConversionSupportGrant = false
            };
            var projectId = "project-123";
            var result = ConversionTasks.BuildTaskList(conversionTaskList, projectId).ProjectKickoffTasks;

            result.Should().HaveCountGreaterThan(1).And.BeInAscendingOrder(x => x.DisplayOrder);
            result.Should().NotContain(x => x.Name == "Process conversion support grant");
            result.Should().OnlyHaveUniqueItems(x => x.DisplayOrder);
        }
    }
}
