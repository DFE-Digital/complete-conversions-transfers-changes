using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeedOfTerminationForMasterFundingAgreement
{
    public class DeedOfTerminationForMasterFundingAgreementModel(ISender sender, IAuthorizationService authorizationService, ILogger<DeedOfTerminationForMasterFundingAgreementModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.DeedOfTerminationForMasterFundingAgreement)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }

        [BindProperty(Name = "contactfinancialreportingteam")]
        public bool? ContactFinancialReportingTeam { get; set; }

        [BindProperty(Name = "signedsecretarystate")]
        public bool? SignedSecretaryState { get; set; }

        [BindProperty(Name = "savedacademysharepointholder")]
        public bool? SavedAcademySharePointHolder { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = TransferTaskData.DeedOfTerminationForTheMasterFundingAgreementNotApplicable;
            Received = TransferTaskData.DeedOfTerminationForTheMasterFundingAgreementReceived;
            Signed = TransferTaskData.DeedOfTerminationForTheMasterFundingAgreementSigned;
            Saved = TransferTaskData.DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint;
            Cleared = TransferTaskData.DeedOfTerminationForTheMasterFundingAgreementCleared;
            ContactFinancialReportingTeam = TransferTaskData.DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam;
            SignedSecretaryState = TransferTaskData.DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState;
            SavedAcademySharePointHolder = TransferTaskData.DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateDeedOfTerminationMasterFundingAgreementTaskCommand(new Domain.ValueObjects.TaskDataId(TasksDataId.GetValueOrDefault())!, Received, Cleared, Saved, Signed, ContactFinancialReportingTeam, SignedSecretaryState, SavedAcademySharePointHolder, NotApplicable));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
