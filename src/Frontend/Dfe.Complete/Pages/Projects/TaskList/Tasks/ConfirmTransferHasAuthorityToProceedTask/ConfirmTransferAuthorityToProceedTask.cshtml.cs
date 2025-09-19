using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmTransferHasAuthorityToProceedTask
{
    public class ConfirmTransferHasAuthorityToProceedTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmTransferHasAuthorityToProceedTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmTransferHasAuthorityToProceed)
    {
        public bool? Sent { get; set; }
        [BindProperty(Name = "any-information-changed")]
        public bool? AnyInformationChanged { get; set; }

        [BindProperty(Name = "baseline-sheet-approved")]
        public bool? BaselineSheetApproved { get; set; }

        [BindProperty(Name = "confirm-to-proceed")]
        public bool? ConfirmToProceed { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync(); 
            TasksDataId = Project.TasksDataId?.Value;
            if (Project.Type == ProjectType.Transfer)
            {
                BaselineSheetApproved = TransferTaskData.ConditionsMetBaselineSheetApproved;
                AnyInformationChanged = TransferTaskData.ConditionsMetCheckAnyInformationChanged;
                ConfirmToProceed = Project.AllConditionsMet;
            } 
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await sender.Send(new UpdateConfirmTransferHasAuthorityToProceedTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, AnyInformationChanged, BaselineSheetApproved, ConfirmToProceed));
            TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
