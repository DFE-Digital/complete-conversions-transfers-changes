using Dfe.Complete.Application.Projects.Queries.ProjectsByLocalAuthority;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectLocalAuthoritiesQueryService(CompleteContext context)
{
    public IQueryable<ListAllLocalAuthorities> ListAllProjectLocalAuthorities(ProjectState? projectStatus,
        ProjectType? type)
    {
        var query = context.Projects
            .Where(project => projectStatus == null || project.State == projectStatus)
            .Where(project => type == null || type == project.Type)
            .Include(p => p.AssignedTo)
            .Join(context.LocalAuthorities, project => project.local)
            
        return null; 
    }
}