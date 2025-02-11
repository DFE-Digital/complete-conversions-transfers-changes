using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models; 
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectLocalAuthoritiesQueryService(CompleteContext context)
    : IListAllProjectLocalAuthoritiesQueryService
{
    public IQueryable<ListAllProjectLocalAuthoritiesQueryModel> ListAllProjectLocalAuthorities(
        ProjectState? projectStatus,
        ProjectType? type)
    {
        var query = context.Projects
            .Where(project => projectStatus == null || project.State == projectStatus)
            .Where(project => type == null || type == project.Type)
            .Include(p => p.AssignedTo)
            .Join(context.LocalAuthorities,
                project => project.LocalAuthorityId,
                localAuthority => localAuthority.Id,
                (project, authority) => new ListAllProjectLocalAuthoritiesQueryModel(authority, project));

        return query;
    }
}