using Dfe.Complete.Application.Projects.Queries.ProjectsByLocalAuthority;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectLocalAuthoritiesQueryService(CompleteContext context)
{
    public IQueryable<ListAllLocalAuthorities> ListAllProjectLocalAuthorities(ProjectState? projectStatus,
        ProjectType? projectType)
    {
        
        

        return null; 
    }
}