using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using System.Security.Claims;

namespace Dfe.Complete.Services
{
    public interface IProjectPermissionService
    {
        bool UserCanView(ProjectDto project, ClaimsPrincipal user);
        bool UserCanComplete(ProjectDto project, ClaimsPrincipal user);
        bool UserCanEdit(ProjectDto project, ClaimsPrincipal user);
        bool UserCanDaoRevocation(ProjectDto project, ClaimsPrincipal user);
        bool UserIsAdmin(ProjectDto project, ClaimsPrincipal user);
    }

    public class ProjectPermissionService : IProjectPermissionService
    {
        public bool UserCanView(ProjectDto project, ClaimsPrincipal user) =>
            UserIsServiceSupport(user) || ProjectIsInViewState(project);

        public bool UserCanComplete(ProjectDto project, ClaimsPrincipal user) =>
            ProjectIsActive(project) && (UserIsServiceSupport(user) || UserIsAssignee(project, user));

        public bool UserCanEdit(ProjectDto project, ClaimsPrincipal user) =>
            UserIsServiceSupport(user) || (ProjectIsActive(project) && UserIsAssignee(project, user));
        public bool UserCanDaoRevocation(ProjectDto project, ClaimsPrincipal user) =>
            ProjectIsActive(project) && (UserIsServiceSupport(user) || UserIsAssignee(project, user)) && project.DirectiveAcademyOrder == true;

        public bool UserIsAdmin(ProjectDto project, ClaimsPrincipal user) =>
            UserIsServiceSupport(user);

        private static bool ProjectIsActive(ProjectDto project) => project.State == ProjectState.Active;
        private static bool ProjectIsInViewState(ProjectDto project) => new List<ProjectState> { ProjectState.Active, ProjectState.Completed, ProjectState.DaoRevoked, ProjectState.Inactive }.Contains(project.State);
        private static bool UserIsAssignee(ProjectDto project, ClaimsPrincipal user) => project.AssignedToId?.Value == user.GetUserId().Value;
        private static bool UserIsServiceSupport(ClaimsPrincipal user) => user.IsInRole(UserRolesConstants.ServiceSupport);
    }
}
