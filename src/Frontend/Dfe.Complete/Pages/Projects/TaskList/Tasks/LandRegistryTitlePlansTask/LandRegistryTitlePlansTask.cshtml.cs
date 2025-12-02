using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.LandRegistryTitlePlansTask
{
    public class LandRegistryTitlePlansTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<LandRegistryTitlePlansTaskModel> logger, IProjectPermissionService projectPermissionService)
        : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.LandRegistryTitlePlans, projectPermissionService)
    {
        [BindProperty]
        public bool? Cleared { get; set; }

        [BindProperty]
        public bool? Received { get; set; }

        [BindProperty]
        public bool? Saved { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            Cleared = ConversionTaskData.LandRegistryCleared;
            Received = ConversionTaskData.LandRegistryReceived;
            Saved = ConversionTaskData.LandRegistrySaved;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateLandRegistryTitlePlansTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Received, Cleared, Saved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
