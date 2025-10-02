using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.MainContactTask
{
    public class MainContactTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<MainContactTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.MainContact)
    {
        [BindProperty]
        public Guid? MainContactId { get; set; }  

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            MainContactId = Project.MainContactId?.Value; 
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await sender.Send(new UpdateMainContactTaskCommand(new ProjectId(Guid.Parse(ProjectId)), new ContactId(MainContactId!.Value)));
            TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
