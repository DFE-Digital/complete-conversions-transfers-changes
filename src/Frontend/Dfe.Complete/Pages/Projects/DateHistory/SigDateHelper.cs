using System.Security.Claims;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Extensions;
using Dfe.Complete.Extensions;

namespace Dfe.Complete.Pages.Projects.DateHistory;

public static class SigDateHelper
{
    public static bool CanEditSignificantDate(ProjectDto project, ClaimsPrincipal user, ProjectTeam currentUserTeam)
    {
        var projectIsAssignedToUser = project.AssignedToId == user.GetUserId();
        var significantDateIsConfirmed = project.SignificantDateProvisional is false;

        return significantDateIsConfirmed && (projectIsAssignedToUser || currentUserTeam.TeamIsServiceSupport());
    }
}