using Dfe.Complete.Application.Projects.Models;

namespace Dfe.Complete.Models
{
    public static class TaskListViewModel
    {
        public static TaskListStatus HandoverWithRegionalDeliveryOfficerTaskStatus(TaskDataModel taskDataModel)
        { 
            if (!taskDataModel.HandoverReview.HasValue &&
                !taskDataModel.HandoverMeeting.HasValue &&
                !taskDataModel.HandoverNotes.HasValue &&
                !taskDataModel.HandoverNotApplicable.HasValue) 
            {
                return TaskListStatus.NotStarted;
            }
            if (taskDataModel.HandoverNotApplicable == true)
            {
                return TaskListStatus.NotApplicable;
            }

            if (taskDataModel.HandoverReview.Equals(true) &&
                taskDataModel.HandoverMeeting.Equals(true) &&
                taskDataModel.HandoverNotes.Equals(true))
            {
                return TaskListStatus.Completed;
            }

            return TaskListStatus.InProgress;
        }
    }
}
