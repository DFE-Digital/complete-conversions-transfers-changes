using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.PrivateFinanceInitiativeTask
{
    public class PrivateFinanceInitiativeTaskModel(
        ISender sender,
        IAuthorizationService authorizationService,
        ILogger<PrivateFinanceInitiativeTaskModel> logger,
        IProjectPermissionService projectPermissionService)
        : BaseProjectTaskModel(
            sender,
            authorizationService,
            logger,
            NoteTaskIdentifier.PrivateFinanceInitiative,
            projectPermissionService)
    {
        [BindProperty]
        public bool? NotApplicable { get; set; }

        [BindProperty]
        public bool? SupplementaryFundingAgreementPfiClausesInserted { get; set; }

        [BindProperty]
        public bool? MasterFundingAgreementPfiClausesInserted { get; set; }

        [BindProperty]
        public bool? Received { get; set; }

        [BindProperty]
        public bool? DocumentsSentToSOPUForClearance { get; set; }

        [BindProperty]
        public bool? Cleared { get; set; }

        [BindProperty]
        public bool? DraftSaved { get; set; }
        
        [BindProperty]
        public bool? SignedByAllStakeHolders { get; set; }

        [BindProperty]
        public bool? FinalVersionSavedInSharepointFolder { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
            {
                return Redirect(RouteConstants.ErrorPage);
            }

            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = ConversionTaskData.PrivateFinanceInitiativeNotApplicable;
            SupplementaryFundingAgreementPfiClausesInserted = ConversionTaskData.PrivateFinanceInitiativeSupplementaryFundingAgreementPfiClausesInserted;
            MasterFundingAgreementPfiClausesInserted = ConversionTaskData.PrivateFinanceInitiativeMasterFundingAgreementPfiClausesInserted;

            Received = ConversionTaskData.PrivateFinanceInitiativeReceived;
            DocumentsSentToSOPUForClearance = ConversionTaskData.PrivateFinanceInitiativeDocumentsSentToSOPUForClearance;
            Cleared = ConversionTaskData.PrivateFinanceInitiativeCleared;
            DraftSaved = ConversionTaskData.PrivateFinanceInitiativeDraftSavedInTrustSharepointFolder;
            SignedByAllStakeHolders = ConversionTaskData.PrivateFinanceInitiativeSignedByAllStakeholders;
            FinalVersionSavedInSharepointFolder = ConversionTaskData.PrivateFinanceInitiativeFinalVersionSavedInSharepointFolder;
            
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdatePrivateFinanceInitiativeTaskCommand(
                new TaskDataId(TasksDataId.GetValueOrDefault())!,
                NotApplicable,
                SupplementaryFundingAgreementPfiClausesInserted,
                MasterFundingAgreementPfiClausesInserted,
                Received,
                DocumentsSentToSOPUForClearance,
                Cleared,
                DraftSaved,
                SignedByAllStakeHolders,
                FinalVersionSavedInSharepointFolder));

            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
