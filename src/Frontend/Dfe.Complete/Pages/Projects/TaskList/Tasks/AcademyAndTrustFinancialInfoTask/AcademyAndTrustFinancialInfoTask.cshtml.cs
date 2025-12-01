using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.AcademyAndTrustFinancialInfoTask
{
    public class AcademyAndTrustFinancialInfoTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<AcademyAndTrustFinancialInfoTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.CheckAndConfirmAcademyAndTrustFinancialInformation, projectPermissionService)
    {
        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty]
        public string? AcademySurplusOrDeficit { get; set; }
        [BindProperty]
        public string? TrustSurplusOrDeficit { get; set; }
        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public IEnumerable<AcademyAndTrustFinancialStatus> FinancialStatuses { get; set; } =
            new List<AcademyAndTrustFinancialStatus>();

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);         
            
            FinancialStatuses = EnumExtensions.ToList<AcademyAndTrustFinancialStatus>();

            TasksDataId = Project.TasksDataId?.Value;
            NotApplicable = TransferTaskData.CheckAndConfirmFinancialInformationNotApplicable;
            AcademySurplusOrDeficit = TransferTaskData.CheckAndConfirmFinancialInformationAcademySurplusDeficit;
            TrustSurplusOrDeficit = TransferTaskData.CheckAndConfirmFinancialInformationTrustSurplusDeficit;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {

            var academySurplusOrDeficit = AcademySurplusOrDeficit == null ? null : EnumExtensions.FromDescriptionValue<AcademyAndTrustFinancialStatus>(AcademySurplusOrDeficit);
            var trustSurplusOrDeficit = TrustSurplusOrDeficit == null ? null : EnumExtensions.FromDescriptionValue<AcademyAndTrustFinancialStatus>(TrustSurplusOrDeficit);

            await Sender.Send(new UpdateAcademyAndTrustFinancialInformationTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, academySurplusOrDeficit, trustSurplusOrDeficit));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
