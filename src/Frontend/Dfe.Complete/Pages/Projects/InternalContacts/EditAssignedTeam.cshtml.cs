using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.InternalContacts;

public class EditAssignedTeam(ISender sender, ErrorService errorService, ILogger<InternalContacts> logger) : BaseProjectPageModel(sender)
{
    [BindProperty]
    [Required]
    public ProjectTeam? Team { get; set; } = default!;
    
    public override async Task<IActionResult> OnGet()
    {
        await base.OnGet();
        Team = Project.Team;
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        await UpdateCurrentProject();
        
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return await OnGet();
        }
        var updateRequest = new UpdateAssignedTeamCommand(Project.Urn, Team);
        await sender.Send(updateRequest);
        TempData.SetNotification(NotificationType.Success, "Success", "Project has been assigned to team successfully");
        return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectInternalContacts));

    }
}