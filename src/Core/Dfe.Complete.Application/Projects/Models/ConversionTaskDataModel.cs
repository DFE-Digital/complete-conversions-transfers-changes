using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models
{
    public record ConversionTaskDataModel(TaskDataId TaskDataId,
        bool? HandoverReview,
        bool? HandoverNotes,
        bool? HandoverMeeting,
        bool? HandoverNotApplicable)
    {
        public static ConversionTaskDataModel MapConversionTaskDataToModel(ConversionTasksData taskData)
        {
            return new ConversionTaskDataModel(
                taskData.Id,
                taskData.HandoverReview,
                taskData.HandoverNotes,
                taskData.HandoverMeeting,
                taskData.HandoverNotApplicable);
        }
    }
}
