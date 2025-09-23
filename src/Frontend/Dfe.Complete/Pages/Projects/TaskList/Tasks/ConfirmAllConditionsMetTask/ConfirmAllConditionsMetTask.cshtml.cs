using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.ArticlesOfAssociationTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmAllConditionsMetTask
{
    public class ConfirmAllConditionsMetTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ArticlesOfAssociationTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmAllConditionsMet)
    {
        [BindProperty(Name = "confirm")] 
        public bool? Confirm { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Confirm = Project.AllConditionsMet;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await sender.Send(new UpdateConfirmAllConditionsMetTaskCommand(new ProjectId(Guid.Parse(ProjectId)), Confirm));
            TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
