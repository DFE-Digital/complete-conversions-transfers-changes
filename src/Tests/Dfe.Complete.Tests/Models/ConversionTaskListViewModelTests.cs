using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;

namespace Dfe.Complete.Tests.Models
{
    public class ConversionTaskListViewModelTests
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
            var model = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                HandoverReview = handoverReview,
                HandoverNotes = handoverNotes,
                HandoverMeeting = handoverMeeting,
                HandoverNotApplicable = handoverNotApplicable
            };
            var result = ConversionTaskListViewModel.HandoverWithRegionalDeliveryOfficerTaskStatus(model);
            Assert.Equal(taskListStatus, result);
        }

        [Theory]
        [InlineData(false, null, null, null, null, null, TaskListStatus.NotStarted)]
        [InlineData(true, false, null, false, null, false, TaskListStatus.InProgress)] 
        [InlineData(false, false, false, false, false, false, TaskListStatus.NotStarted)]
        [InlineData(true, true, false, false, false, false, TaskListStatus.InProgress)]
        [InlineData(true, null, null, null, null, null, TaskListStatus.InProgress)]
        [InlineData(true, true, true, true, true, true, TaskListStatus.InProgress)]
        [InlineData(false, true, true, true, true, true, TaskListStatus.Completed)] 
        public void ExternalStakeHolderKickoffTaskStatus_ShouldReturn_CorrectStatus(
            bool? significantDateProvisional,
            bool? introEmails,
            bool? proforma,
            bool? setupMeeting,
            bool? meeting,
            bool? checkProvisionalDate,
            TaskListStatus expectedStatus)
        {
            var taskData = new ConversionTaskDataDto
            {
                Id = new TaskDataId(Guid.NewGuid()),
                StakeholderKickOffIntroductoryEmails = introEmails,
                StakeholderKickOffLocalAuthorityProforma = proforma,
                StakeholderKickOffSetupMeeting = setupMeeting,
                StakeholderKickOffMeeting = meeting,
                StakeholderKickOffCheckProvisionalConversionDate = checkProvisionalDate
            };

            var project = new ProjectDto
            {
                SignificantDateProvisional = significantDateProvisional
            };

            var result = ConversionTaskListViewModel.ExternalStakeHolderKickoffTaskStatus(taskData, project);

            Assert.Equal(expectedStatus, result);
        }

    }
}
