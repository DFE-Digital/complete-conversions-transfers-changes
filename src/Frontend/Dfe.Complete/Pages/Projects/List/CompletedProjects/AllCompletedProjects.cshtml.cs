using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Constants;

using MediatR;

namespace Dfe.Complete.Pages.Projects.
List.CompletedProjects;

public class AllCompletedProjectsViewModel(ISender sender) : AllProjectsModel(CompletedNavigation)
{
    public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;

        var listProjectQuery = new ListAllProjectsQuery(ProjectState.Completed, null, PageNumber - 1, PageSize);

        var listResponse = await sender.Send(listProjectQuery);
        Projects = (listResponse.Value ?? [])
            .Where(p => p.ProjectCompletionDate != null)
            .OrderByDescending(p => p.ProjectCompletionDate)
            .ToList();

        var countProjectQuery = new CountAllProjectsQuery(ProjectState.Completed, null);
        var countResponse = await sender.Send(countProjectQuery);

        Pagination = new PaginationModel(RouteConstants.CompletedProjects, PageNumber, countResponse.Value, PageSize);
    }

    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}
