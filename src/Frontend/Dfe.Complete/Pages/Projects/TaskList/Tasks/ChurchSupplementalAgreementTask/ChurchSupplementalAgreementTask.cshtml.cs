using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ChurchSupplementalAgreementTask
{
    public class ChurchSupplementalAgreementTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ChurchSupplementalAgreementTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ChurchSupplementalAgreement)
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
        public bool? SentOrSaved { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            if (Project.Type == ProjectType.Transfer)
            {
                NotApplicable = TransferTaskData.ChurchSupplementalAgreementNotApplicable;
                Received = TransferTaskData.ChurchSupplementalAgreementReceived;
                Cleared = TransferTaskData.ChurchSupplementalAgreementCleared;
                Signed = TransferTaskData.ChurchSupplementalAgreementSignedIncomingTrust;
                SignedByDiocese = TransferTaskData.ChurchSupplementalAgreementSignedDiocese;
                Saved = TransferTaskData.ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese;
                SentOrSaved = TransferTaskData.ChurchSupplementalAgreementSavedAfterSigningBySecretaryState;
                SignedBySecretaryState = TransferTaskData.ChurchSupplementalAgreementSignedSecretaryState;
            }
            else
            {
                NotApplicable = ConversionTaskData.ChurchSupplementalAgreementNotApplicable;
                Received = ConversionTaskData.ChurchSupplementalAgreementReceived;
                Cleared = ConversionTaskData.ChurchSupplementalAgreementCleared;
                Signed = ConversionTaskData.ChurchSupplementalAgreementSigned;
                SignedByDiocese = ConversionTaskData.ChurchSupplementalAgreementSignedDiocese;
                Saved = ConversionTaskData.ChurchSupplementalAgreementSaved;
                SentOrSaved = ConversionTaskData.ChurchSupplementalAgreementSent;
                SignedBySecretaryState = ConversionTaskData.ChurchSupplementalAgreementSignedSecretaryState;
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateChurchSupplementalAgreementTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type,
                NotApplicable, Received, Cleared, Signed, SignedByDiocese, Saved, SignedBySecretaryState, SentOrSaved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
