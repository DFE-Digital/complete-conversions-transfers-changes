using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByLocalAuthority;

public class AllLocalAuthorities(ISender sender) : AllProjectsModel(ByLocalAuthorityNavigation)
{
    public List<ListAllProjectLocalAuthoritiesResultModel> LocalAuthorities { get; set; }
    
    public async Task OnGet()
    {
        var query = new ListAllProjectsByLocalAuthoritiesQuery
            { Count = PageSize, Page = PageNumber - 1, State = ProjectState.Active };

        var listResponse = await sender.Send(query);

        LocalAuthorities = listResponse.Value ?? [];

        Pagination = new PaginationModel("/projects/all/local-authorities", PageNumber, listResponse.ItemCount,
            PageSize);
    }
}