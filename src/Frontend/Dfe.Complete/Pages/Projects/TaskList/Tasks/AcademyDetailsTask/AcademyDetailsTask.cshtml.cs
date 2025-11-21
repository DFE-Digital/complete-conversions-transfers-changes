using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.AcademyDetailsTask
{
    public class AcademyDetailsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<AcademyDetailsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.AcademyDetails)
    {
        [BindProperty(Name = "academyname")]
        [DisplayName("Name")]
        public string? AcademyName { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            AcademyName = ConversionTaskData.AcademyDetailsName;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateConfirmAcademyNameTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, AcademyName));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
