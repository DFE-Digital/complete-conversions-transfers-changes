using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.RequestNewUrnAndRecordForAcademyTask
{
    public class RequestNewUrnAndRecordForAcademyTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<RequestNewUrnAndRecordForAcademyTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.RequestNewUrnAndRecordForAcademy, projectPermissionService)
    {
        [BindProperty]
        public Guid? TasksDataId { get; set; }

        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "complete")]
        public bool? Complete { get; set; }

        [BindProperty(Name = "receive")]
        public bool? Receive { get; set; }

        [BindProperty(Name = "give")]
        public bool? Give { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;

            NotApplicable = TransferTaskData.RequestNewUrnAndRecordNotApplicable;
            Complete = TransferTaskData.RequestNewUrnAndRecordComplete;
            Receive = TransferTaskData.RequestNewUrnAndRecordReceive;
            Give = TransferTaskData.RequestNewUrnAndRecordGive;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateRequestNewUrnAndRecordForAcademyTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, NotApplicable, Complete, Receive, Give));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
