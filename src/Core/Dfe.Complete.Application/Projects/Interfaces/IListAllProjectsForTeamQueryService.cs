using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsForTeamQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjectsForTeam(ProjectTeam team, ProjectState? projectStatus, ProjectType? type);
}