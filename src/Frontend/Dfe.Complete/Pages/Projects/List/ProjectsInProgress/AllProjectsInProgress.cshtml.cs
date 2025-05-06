using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress
{
    public class ProjectsInProgressInProgressViewModel(ISender sender) : ProjectsInProgressModel(AllProjectSubNavigation)
    {
        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

        public async Task OnGet()
        {
            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
            
            var listProjectQuery = new ListAllProjectsQuery(ProjectState.Active, null, PageNumber-1, PageSize);

            var listResponse = await sender.Send(listProjectQuery);
            Projects = listResponse.Value ?? [];
            
            var countProjectQuery = new CountAllProjectsQuery(ProjectState.Active, null);
            var countResponse = await sender.Send(countProjectQuery);

            Pagination = new PaginationModel("/projects/all/in-progress/all", PageNumber, countResponse.Value, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}