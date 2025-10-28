using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models; 
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.DeleteProject
{
    public class ConfirmDeleteProjectModel(ISender sender, ILogger<ConfirmDeleteProjectModel> logger) : BaseProjectPageModel(sender, logger)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await UpdateCurrentProject();
            await SetEstablishmentAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        { 
            await Sender.Send(new UpdateDeleteProjectCommand(new Domain.ValueObjects.ProjectId(Guid.Parse(ProjectId))));
            TempData.SetNotification(
                   NotificationType.Success,
                   "Success",
                   "The project was deleted."
               );
            return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectsInProgress));
        }
    }
}
