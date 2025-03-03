using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsForLocalAuthorityQueryService(CompleteContext context) : IListAllProjectsForLocalAuthorityQueryService
{
    public IQueryable<ListAllProjectsQueryModel> ListAllProjectsForLocalAuthority(string localAuthorityCode,
        ProjectState? projectStatus, ProjectType? type)
    {
        var query = context.Projects
            .Where(project => projectStatus == null || project.State == projectStatus)
            .Where(project => type == null || type == project.Type)
            .Include(p => p.AssignedTo)
            .Join(context.GiasEstablishments.Where(e => e.LocalAuthorityCode == localAuthorityCode),
                project => project.Urn, establishment => establishment.Urn,
                (project, establishment) => new ListAllProjectsQueryModel(project, establishment));

        return query;
    }
}