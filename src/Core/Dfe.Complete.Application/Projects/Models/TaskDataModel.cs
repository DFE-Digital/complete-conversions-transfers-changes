using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models
{
    public record TaskDataModel(TaskDataId TaskDataId,
        bool? HandoverReview,
        bool? HandoverNotes,
        bool? HandoverMeeting,
        bool? HandoverNotApplicable)
    {
        public static TaskDataModel MapConversionTaskDataToModel(ConversionTasksData taskData)
        {
            return new TaskDataModel(
                taskData.Id,
                taskData.HandoverReview,
                taskData.HandoverNotes,
                taskData.HandoverMeeting,
                taskData.HandoverNotApplicable);
        }
        public static TaskDataModel MapTransferTaskDataToModel(TransferTasksData taskData)
        {
            return new TaskDataModel(
                taskData.Id,
                taskData.HandoverReview,
                taskData.HandoverNotes,
                taskData.HandoverMeeting,
                taskData.HandoverNotApplicable);
        }
    }
}
