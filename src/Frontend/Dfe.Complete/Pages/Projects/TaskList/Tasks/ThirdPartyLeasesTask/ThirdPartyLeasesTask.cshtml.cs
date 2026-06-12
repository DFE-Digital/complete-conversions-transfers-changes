using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ThirdPartyLeasesTask
{
    public class ThirdPartyLeasesTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ThirdPartyLeasesTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ThirdPartyLeases, projectPermissionService)
    {
        [BindProperty]
        public Guid? TasksDataId { get; set; }

        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "email")]
        public bool? Email { get; set; }

        [BindProperty(Name = "receive")]
        public bool? Receive { get; set; }

        [BindProperty(Name = "save")]
        public bool? Save { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            NotApplicable = ConversionTaskData.ThirdPartyLeasesNotApplicable;
            Email = ConversionTaskData.ThirdPartyLeasesEmail;
            Receive = ConversionTaskData.ThirdPartyLeasesReceive;
            Save = ConversionTaskData.ThirdPartyLeasesSave;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(
                new UpdateThirdPartyLeasesTaskCommand(
                    new TaskDataId(TasksDataId.GetValueOrDefault())!, 
                    NotApplicable, 
                    Email, 
                    Receive, 
                    Save
                )
            );
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}