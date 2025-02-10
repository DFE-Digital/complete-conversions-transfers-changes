using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Queries.ProjectsByLocalAuthority;
using Dfe.Complete.Domain.Enums;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByLocalAuthority;

public class AllLocalAuthorities(ISender sender) : AllProjectsModel(ByLocalAuthorityNavigation)
{
    
    
    public async Task OnGet()
    {
        var query = new ListAllProjectLocalAuthoritiesQuery(ProjectState.Active, null);

        var result = await sender.Send(query);
    }
}