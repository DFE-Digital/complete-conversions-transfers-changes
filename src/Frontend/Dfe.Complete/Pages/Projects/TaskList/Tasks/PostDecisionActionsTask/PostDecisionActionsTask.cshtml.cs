using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.PostDecisionActionsTask
{
    public class PostDecisionActionsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<PostDecisionActionsTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.PostDecisionActions, projectPermissionService)
    {
        [BindProperty]
        public bool? ApplicationUploaded { get; set; }
        
        [BindProperty]
        public bool? AcademyOrderUploaded { get; set; }
        
        [BindProperty]
        public bool? LaProformaUploaded { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            TasksDataId = Project.TasksDataId?.Value;
            ApplicationUploaded = ConversionTaskData.PostDecisionActionsApplicationUploaded;
            AcademyOrderUploaded = ConversionTaskData.PostDecisionActionsAcademyOrderUploaded;
            LaProformaUploaded = ConversionTaskData.PostDecisionActionsLaProformaUploaded;
            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdatePostDecisionActionsTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, ApplicationUploaded, AcademyOrderUploaded, LaProformaUploaded));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}