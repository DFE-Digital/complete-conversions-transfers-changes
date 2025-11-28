using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.IncomingTrustHasCompletedAllActionsTask
{
    public class IncomingTrustHasCompletedAllActionsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<IncomingTrustHasCompletedAllActionsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmIncomingTrustHasCompletedAllActions)
    {
        [BindProperty]
        public Guid? TasksDataId { get; set; }

        [BindProperty(Name = "emailed")]
        public bool? Emailed { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            Emailed = TransferTaskData.ConfirmIncomingTrustHasCompletedAllActionsEmailed;
            Saved = TransferTaskData.ConfirmIncomingTrustHasCompletedAllActionsSaved;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateConfirmIncomingTrustHasCompleteAllActionsTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Emailed, Saved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
