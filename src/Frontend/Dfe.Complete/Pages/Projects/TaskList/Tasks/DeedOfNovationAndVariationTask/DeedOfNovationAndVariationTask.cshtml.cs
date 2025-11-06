using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DeedOfNovationAndVariationTask
{
    public class DeedOfNovationAndVariationTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<HandoverWithDeliveryOfficerTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.DeedOfNovationAndVariation)
    {
        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "signed_outgoing_trust")]
        public bool? SignedOutgoingTrust { get; set; }

        [BindProperty(Name = "signed_incoming_trust")]
        public bool? SignedIncomingTrust { get; set; }

        [BindProperty(Name = "save_after_sign")]
        public bool? SaveAfterSign { get; set; }

        [BindProperty(Name = "signed_secretary_state")]
        public bool? SignedSecretaryState { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            SaveAfterSign = TransferTaskData.DeedOfNovationAndVariationSaveAfterSign;
            Received = TransferTaskData.DeedOfNovationAndVariationReceived;
            Cleared = TransferTaskData.DeedOfNovationAndVariationCleared;
            Saved = TransferTaskData.DeedOfNovationAndVariationSaved;
            SignedOutgoingTrust = TransferTaskData.DeedOfNovationAndVariationSignedOutgoingTrust;
            SignedIncomingTrust = TransferTaskData.DeedOfNovationAndVariationSignedIncomingTrust;
            SignedSecretaryState = TransferTaskData.DeedOfNovationAndVariationSignedSecretaryState;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await sender.Send(new UpdateDeedOfNovationAndVariationTaskCommand(
                new TaskDataId(TasksDataId.GetValueOrDefault())!, Received, Cleared, SignedOutgoingTrust, SignedIncomingTrust, Saved, SignedSecretaryState, SaveAfterSign));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
