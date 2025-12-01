using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Services;
using System.Security.Claims;

namespace Dfe.Complete.Pages.Projects.DateHistory;

public static class SigDateHelper
{
    public static bool CanEditSignificantDate(ProjectDto project, ClaimsPrincipal user, ProjectTeam currentUserTeam, IProjectPermissionService projectPermissionService)
    {
        var significantDateIsConfirmed = project.SignificantDateProvisional is false;

        return significantDateIsConfirmed && projectPermissionService.UserCanEdit(project, user);
    }
}