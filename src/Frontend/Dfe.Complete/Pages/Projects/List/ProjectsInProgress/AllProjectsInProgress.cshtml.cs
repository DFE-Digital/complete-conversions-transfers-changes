using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.CountProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress
{
    public class ProjectsInProgressInProgressViewModel(ISender sender) : ProjectsInProgressViewModel
    {
        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

        public async Task OnGet()
        {

            var listProjectQuery = new ListAllProjectsQuery(ProjectState.Active, null, null, PageNumber-1, PageSize);

            var listResponse = await sender.Send(listProjectQuery);
            Projects = listResponse.Value ?? [];
            
            var countProjectQuery = new CountProjectQuery(ProjectState.Active, null, null);
            var countResponse = await sender.Send(countProjectQuery);

            Pagination = new PaginationModel("/projects/all/in-progress/all" ,PageNumber, countResponse.Value, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}