using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.DBSChecksTask
{
    public class ConfirmDbsChecksModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmDbsChecksModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmDBSChecks, projectPermissionService)
    {

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        [BindProperty]
        public bool? ConfirmDBSChecks { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            ConfirmDBSChecks = ConversionTaskData.ConfirmDBSChecks;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateConfirmDbsChecksTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault()), ConfirmDBSChecks));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
