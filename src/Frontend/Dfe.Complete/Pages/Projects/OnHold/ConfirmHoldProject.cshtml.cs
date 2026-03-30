using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Pages.Projects.OnHold;

[ExcludeFromCodeCoverage]
public class ConfirmHoldProjectModel(ISender sender, ILogger<ConfirmHoldProjectModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectPageModel(sender, logger, projectPermissionService)
{
    public override async Task<IActionResult> OnGetAsync()
    {
        await UpdateCurrentProject();

        if ( Project.State != Domain.Enums.ProjectState.Active)
        {
            return Redirect(string.Format(RouteConstants.Project, ProjectId));
        }
        await SetEstablishmentAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await sender.Send(new UpdateProjectOnHoldCommand(new ProjectId(new Guid(ProjectId))), HttpContext.RequestAborted);
        if (result.IsSuccess)
        {
            TempData.SetNotification(
                NotificationType.Success,
                "Success",
                "The project was put on hold."
            );

            return Redirect(string.Format(RouteConstants.Project, ProjectId));
        }

        return Page();
    }
}