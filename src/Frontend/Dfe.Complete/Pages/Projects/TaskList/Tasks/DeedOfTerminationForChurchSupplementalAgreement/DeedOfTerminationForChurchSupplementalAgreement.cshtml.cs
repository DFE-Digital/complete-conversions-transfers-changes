using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeedOfTerminationForChurchSupplementalAgreement
{
    public class DeedOfTerminationForChurchSupplementalAgreementModel(ISender sender, IAuthorizationService authorizationService, ILogger<DeedOfTerminationForChurchSupplementalAgreementModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.DeedOfTerminationForChurchSupplementalAgreement)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty]
        public bool? Received { get; set; }

        [BindProperty]
        public bool? Cleared { get; set; }

        [BindProperty]
        public bool? Signed { get; set; }

        [BindProperty]
        public bool? SignedByDiocese { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty]
        public bool? SignedBySecretaryState { get; set; }

        [BindProperty]
        public bool? SavedAfterSigningBySecretaryState { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            TasksDataId = Project.TasksDataId?.Value;
            NotApplicable = TransferTaskData.DeedTerminationChurchAgreementNotApplicable;
            Received = TransferTaskData.DeedTerminationChurchAgreementReceived;
            Cleared = TransferTaskData.DeedTerminationChurchAgreementCleared;
            Signed = TransferTaskData.DeedTerminationChurchAgreementSignedOutgoingTrust;
            SignedByDiocese = TransferTaskData.DeedTerminationChurchAgreementSignedDiocese;
            Saved = TransferTaskData.DeedTerminationChurchAgreementSaved;
            SignedBySecretaryState = TransferTaskData.DeedTerminationChurchAgreementSignedSecretaryState;
            SavedAfterSigningBySecretaryState = TransferTaskData.DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await Sender.Send(new UpdateDeedTerminationChurchSupplementalAgreementTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!,
                NotApplicable, Received, Cleared, Signed, SignedByDiocese, Saved, SignedBySecretaryState, SavedAfterSigningBySecretaryState));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
