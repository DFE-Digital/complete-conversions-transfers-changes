using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Models;

namespace Dfe.Complete.Tests.Models
{
    public class TaskListViewModelTests
    {
        [Theory]
        [InlineData(null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(true, null, null, null, TaskListStatus.NotApplicable)]
        [InlineData(true, true, true, true, TaskListStatus.NotApplicable)]
        [InlineData(false, null, true, null, TaskListStatus.InProgress)]
        [InlineData(null, null, true, null, TaskListStatus.InProgress)]
        [InlineData(null, true, false, true, TaskListStatus.InProgress)]
        [InlineData(false, true, false, true, TaskListStatus.InProgress)]
        [InlineData(false, true, true, false, TaskListStatus.InProgress)]
        [InlineData(false, true, true, true, TaskListStatus.Completed)]
        [InlineData(null, true, true, true, TaskListStatus.Completed)]
        public void HandoverWithRegionalDeliveryOfficerTaskStatus_ShouldReturns_CorrectResult(bool? handoverNotApplicable, bool? handoverReview, bool? handoverNotes, bool? handoverMeeting, TaskListStatus taskListStatus)
        {
            var model = new TaskDataModel(new Domain.ValueObjects.TaskDataId(Guid.NewGuid()),handoverReview, handoverNotes, handoverMeeting, handoverNotApplicable);
            var result = TaskListViewModel.HandoverWithRegionalDeliveryOfficerTaskStatus(model);
            Assert.Equal(taskListStatus, result);
        } 
    }
}
