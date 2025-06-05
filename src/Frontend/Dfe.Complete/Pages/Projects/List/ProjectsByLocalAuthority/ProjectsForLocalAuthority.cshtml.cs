using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.LocalAuthorities.Queries.GetLocalAuthority;
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

    public async Task<IActionResult> OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;

        var localAuthorityQuery = new GetLocalAuthorityByCodeQuery(LocalAuthorityCode.ToString());
        var foundLocalAuthority = await sender.Send(localAuthorityQuery);

        var projectsForLocalAuthorityQuery = new ListAllProjectsForLocalAuthorityQuery(LocalAuthorityCode) { Count = PageSize, Page = PageNumber - 1 };
        var foundProjectsForLocalAuthority = await sender.Send(projectsForLocalAuthorityQuery);

        LocalAuthorityName = foundLocalAuthority?.Value?.Name ?? "";
        Projects = foundProjectsForLocalAuthority.Value ?? [];

        Pagination = new PaginationModel($"/projects/all/local-authorities/{LocalAuthorityCode}", PageNumber,
            foundProjectsForLocalAuthority.ItemCount, PageSize);

        var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage);
        return hasPageFound ?? Page();
    }
}