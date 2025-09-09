using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using System.Security.Claims;

namespace Dfe.Complete.Services
{
    public interface IProjectPermissionService
    {
        bool UserCanEdit(ProjectDto project, ProjectTeam currentUserTeam, ClaimsPrincipal user);
    }

    public class ProjectPermissionService : IProjectPermissionService
    {
        public bool UserCanEdit(ProjectDto project, ProjectTeam currentUserTeam, ClaimsPrincipal user)
        {
            return currentUserTeam == ProjectTeam.ServiceSupport || (project.State != ProjectState.Completed && project.AssignedToId?.Value == user.GetUserId().Value);
        }
    }
}
