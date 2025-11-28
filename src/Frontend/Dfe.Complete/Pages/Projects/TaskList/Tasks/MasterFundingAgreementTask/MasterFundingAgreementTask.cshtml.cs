using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.MasterFundingAgreementTask
{
    public class MasterFundingAgreementTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<MasterFundingAgreementTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.MasterFundingAgreement, projectPermissionService)
    {
        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "signedBySchoolOrTrust")]
        public bool? SignedBySchoolOrTrust { get; set; }

        [BindProperty(Name = "savedInTheSchoolsSharepoint")]
        public bool? SavedInTheSchoolsSharepoint { get; set; }

        [BindProperty(Name = "savedInSchoolAndTrustSharepoint")]
        public bool? SavedInSchoolAndTrustSharepoint { get; set; }

        [BindProperty(Name = "signedOnBehalfOfSeceratyOfState")]
        public bool? SignedOnBehalfOfSeceratyOfState { get; set; }


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
                NotApplicable = TransferTaskData.MasterFundingAgreementNotApplicable;
                Received = TransferTaskData.MasterFundingAgreementReceived;
                Cleared = TransferTaskData.MasterFundingAgreementCleared;
                SignedBySchoolOrTrust = TransferTaskData.MasterFundingAgreementSigned;
                SavedInTheSchoolsSharepoint = TransferTaskData.MasterFundingAgreementSaved;
                SignedOnBehalfOfSeceratyOfState = TransferTaskData.MasterFundingAgreementSignedSecretaryState;
            }
            else
            {
                NotApplicable = ConversionTaskData.MasterFundingAgreementNotApplicable;
                Received = ConversionTaskData.MasterFundingAgreementReceived;
                Cleared = ConversionTaskData.MasterFundingAgreementCleared;
                SignedBySchoolOrTrust = ConversionTaskData.MasterFundingAgreementSigned;
                SavedInTheSchoolsSharepoint = ConversionTaskData.MasterFundingAgreementSaved;
                SavedInSchoolAndTrustSharepoint = ConversionTaskData.MasterFundingAgreementSent;
                SignedOnBehalfOfSeceratyOfState = ConversionTaskData.MasterFundingAgreementSignedSecretaryState;
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateMasterFundingAgreementTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, NotApplicable, Received, Cleared, SignedBySchoolOrTrust, SavedInTheSchoolsSharepoint, SavedInSchoolAndTrustSharepoint, SignedOnBehalfOfSeceratyOfState));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
