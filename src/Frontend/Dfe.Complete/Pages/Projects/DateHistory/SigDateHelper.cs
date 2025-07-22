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
        var projectIsAssignedToUser = project.AssignedToId?.Value.ToString() == user.GetUserAdId();
        var significantDateIsConfirmed = project.SignificantDateProvisional is false;

        var canEditSignificantDate = significantDateIsConfirmed && (projectIsAssignedToUser || currentUserTeam.TeamIsServiceSupport());
        
        return canEditSignificantDate;
    }
}