using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ShareInformationAboutOpeningTask
{
    public class ShareInformationAboutOpeningTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ShareInformationAboutOpeningTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ShareInformationAboutOpening)
    {
        [BindProperty]
        public bool? ShareInformationEmail { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            ShareInformationEmail = ConversionTaskData.ShareInformationEmail;
            return Page();            
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateShareInformationAboutOpeningTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, ShareInformationEmail));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
