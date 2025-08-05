using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models
{
    public record TransferTaskDataModel(TaskDataId TaskDataId,
        bool? HandoverReview,
        bool? HandoverNotes,
        bool? HandoverMeeting,
        bool? HandoverNotApplicable)
    {
        public static TransferTaskDataModel MapTransferTaskDataToModel(TransferTasksData taskData)
        {
            return new TransferTaskDataModel(
                taskData.Id,
                taskData.HandoverReview,
                taskData.HandoverNotes,
                taskData.HandoverMeeting,
                taskData.HandoverNotApplicable);
        }
    }
}
