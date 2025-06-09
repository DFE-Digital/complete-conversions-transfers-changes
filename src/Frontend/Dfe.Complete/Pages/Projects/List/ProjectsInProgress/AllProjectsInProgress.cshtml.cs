using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress
{
    public class ProjectsInProgressInProgressViewModel(ISender sender) : ProjectsInProgressModel(AllProjectSubNavigation)
    {
        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;

            var listProjectQuery = new ListAllProjectsQuery(ProjectState.Active, null, AssignedToState.AssignedOnly, Page: PageNumber - 1, Count: PageSize);

            var listResponse = await sender.Send(listProjectQuery);
            Projects = listResponse.Value ?? [];

            var countProjectQuery = new CountAllProjectsQuery(ProjectState.Active, null, AssignedToState.AssignedOnly);
            var countResponse = await sender.Send(countProjectQuery);

            Pagination = new PaginationModel("/projects/all/in-progress/all", PageNumber, countResponse.Value, PageSize);

            var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage);
            return hasPageFound ?? Page();
        }

        public async Task<IActionResult> OnGetMovePage()
        {
            return await OnGet();
        }
    }
}