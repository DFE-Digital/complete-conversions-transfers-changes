using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsForTeamQueryService(CompleteContext context) : IListAllProjectsForTeamQueryService
{
    public IQueryable<ListAllProjectsQueryModel> ListAllProjectsForTeam(ProjectTeam team, ProjectState? projectStatus, ProjectType? type)
    {
        var query = context.Projects
            .Where(project => projectStatus == null || project.State == projectStatus)
            .Where(project => type == null || type == project.Type)
            .Where(project => project.Team != null && project.Team == team)
            .Include(p => p.AssignedTo)
            .Join(context.GiasEstablishments,
                project => project.Urn, establishment => establishment.Urn,
                (project, establishment) => new ListAllProjectsQueryModel(project, establishment));

        return query;
    }
}