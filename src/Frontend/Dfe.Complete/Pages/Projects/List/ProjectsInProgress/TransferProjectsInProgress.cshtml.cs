using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress
{
    public class TransferProjectsInProgressInProgressModel(ISender sender) : ProjectsInProgressModel(TransfersSubNavigation)
    {

        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

        public async Task OnGet()
        {
            ViewData[TabNavigationModel.ViewDataKey] = _TabNavigationModel;
            var listProjectQuery = new ListAllProjectsQuery(ProjectState.Active, ProjectType.Transfer, PageNumber-1, PageSize);

            var response = await sender.Send(listProjectQuery);
            Projects = response.Value?.ToList() ?? [];
            
            var countProjectQuery = new CountAllProjectsQuery(ProjectState.Active, ProjectType.Transfer);
            var countResponse = await sender.Send(countProjectQuery);

            Pagination = new PaginationModel("/projects/all/in-progress/transfers" ,PageNumber, countResponse.Value, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}