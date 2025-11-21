using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmSchoolHasCompletedAllActions
{
    public class ConfirmSchoolHasCompletedAllActionsModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmSchoolHasCompletedAllActionsModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmSchoolHasCompletedAllActions)
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

            Emailed = ConversionTaskData.SchoolCompletedEmailed;
            Saved = ConversionTaskData.SchoolCompletedSaved;

            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {            
            await Sender.Send(new UpdateConfirmSchoolHasCompletedAllActionsTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Emailed, Saved ));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
