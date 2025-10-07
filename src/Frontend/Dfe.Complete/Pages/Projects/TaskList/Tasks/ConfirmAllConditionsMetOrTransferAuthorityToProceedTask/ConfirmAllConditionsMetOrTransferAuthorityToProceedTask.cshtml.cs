using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmAllConditionsMetOrTransferAuthorityToProceedTask
{
    public class ConfirmAllConditionsMetOrTransferAuthorityToProceedTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmAllConditionsMetOrTransferAuthorityToProceedTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmAllConditionsMet)
    {
        public bool? Sent { get; set; }
        [BindProperty(Name = "any-information-changed")]
        public bool? AnyInformationChanged { get; set; }

        [BindProperty(Name = "baseline-sheet-approved")]
        public bool? BaselineSheetApproved { get; set; }

        [BindProperty(Name = "confirm")]
        public bool? Confirm { get; set; }
        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            TasksDataId = Project.TasksDataId?.Value;
            Type = Project.Type;
            if (Project.Type == ProjectType.Transfer)
            {
                TaskIdentifier = NoteTaskIdentifier.ConfirmTransferHasAuthorityToProceed;
                BaselineSheetApproved = TransferTaskData.ConditionsMetBaselineSheetApproved;
                AnyInformationChanged = TransferTaskData.ConditionsMetCheckAnyInformationChanged;
            }
            Confirm = Project.AllConditionsMet;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await sender.Send(Type == ProjectType.Conversion 
                ? new UpdateConfirmAllConditionsMetTaskCommand(new ProjectId(Guid.Parse(ProjectId)), Confirm)
                : new UpdateConfirmTransferHasAuthorityToProceedTaskCommand(
                    new TaskDataId(TasksDataId.GetValueOrDefault())!, AnyInformationChanged, BaselineSheetApproved, Confirm));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
