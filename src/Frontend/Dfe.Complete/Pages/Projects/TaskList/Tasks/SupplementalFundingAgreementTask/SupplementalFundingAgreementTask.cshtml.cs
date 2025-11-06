using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.SupplementalFundingAgreementTask
{
    public class SupplementalFundingAgreementTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<SupplementalFundingAgreementTaskModel> logger)
        : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.SupplementalFundingAgreement)
    {
        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }

        [BindProperty(Name = "sent")]
        public bool? Sent { get; set; }

        [BindProperty(Name = "signed_secretary_state")]
        public bool? SignedSecretaryState { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            if (Project.Type == ProjectType.Transfer)
            {
                Received = TransferTaskData.SupplementalFundingAgreementReceived;
                Cleared = TransferTaskData.SupplementalFundingAgreementCleared;
                Saved = TransferTaskData.SupplementalFundingAgreementSaved;
            }
            else
            {
                Received = ConversionTaskData.SupplementalFundingAgreementReceived;
                Cleared = ConversionTaskData.SupplementalFundingAgreementCleared;
                Sent = ConversionTaskData.SupplementalFundingAgreementSent;
                Saved = ConversionTaskData.SupplementalFundingAgreementSaved;
                Signed = ConversionTaskData.SupplementalFundingAgreementSigned;
                SignedSecretaryState = ConversionTaskData.SupplementalFundingAgreementSignedSecretaryState;
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await sender.Send(new UpdateSupplementalFundingAgreementTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, Received, Cleared, Sent, Saved, Signed, SignedSecretaryState));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
