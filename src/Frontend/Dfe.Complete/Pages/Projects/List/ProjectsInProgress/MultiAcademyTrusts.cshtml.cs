using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress
{
    public class MultiAcademyTrustsModel(ISender sender) : ProjectsInProgressModel(FormAMatSubNavigation)
    {

        public List<ListMatResultModel> MATS { get; set; } = default!;

        public async Task OnGet()
        {
            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
            var listProjectQuery = new ListAllMaTsQuery(ProjectState.Active) { 
                Page = PageNumber - 1, 
                Count = PageSize
            };

            var response = await sender.Send(listProjectQuery);
            MATS = response.Value?.ToList() ?? [];
            
            Pagination = new PaginationModel("/projects/all/in-progress/form-a-multi-academy-trust", PageNumber, response.ItemCount, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}