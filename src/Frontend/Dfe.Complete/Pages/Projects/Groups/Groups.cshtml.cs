using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Groups
{
    public class ProjectGroupsModel(ISender sender) : BaseProjectsPageModel(String.Empty)
    {
        public List<ListProjectsGroupsModel>? ProjectGroups { get; set; }
        
        public async Task OnGet()
        {
            var request = new GetProjectGroupsQuery() { Page = PageNumber - 1, Count = PageSize };
            var response = await sender.Send(request);
            
            ProjectGroups = response.Value;
            
            Pagination = new PaginationModel($"/groups", PageNumber, ProjectGroups.Count, PageSize);
        }
    }
}
