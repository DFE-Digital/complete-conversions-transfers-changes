using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByLocalAuthority;

public class ProjectsForLocalAuthority(ISender sender) : AllProjectsModel(ByLocalAuthorityNavigation)
{
    [BindProperty(SupportsGet = true)] public string LocalAuthorityCode { get; set; }

    public string LocalAuthorityName { get; set; }
    
    public List<ListAllProjectsResultModel> Projects { get; set; } 

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
        
        var query = new ListAllProjectsForLocalAuthorityQuery(LocalAuthorityCode) { Count = PageSize, Page = PageNumber - 1 };

        var result = await sender.Send(query);

        Projects = result.Value ?? [];
        
        Pagination = new PaginationModel($"/projects/all/local-authorities/{LocalAuthorityCode}", PageNumber,
            result.ItemCount, PageSize);
    }
}