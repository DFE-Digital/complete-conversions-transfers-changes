using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Pages.Projects.ServiceSupport;
using Dfe.Complete.Pages.Projects.ServiceSupport.ConversionURNs;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Groups
{
    public class ProjectGroupsModel(ISender sender) : ServiceSupportModel(ConversionsUrnModel.WithAcademyURNSubNavigation)
    {
        public List<ListProjectsGroupsModel>? ProjectGroups { get; set; }
        
        public async Task OnGet()
        {
            var request = new GetProjectGroupsQuery() { Page = PageNumber - 1, Count = PageSize };
            var response = await sender.Send(request);
            
            ProjectGroups = response.Value;
            
            
            Pagination = new PaginationModel($"/projects/service-support/with-academy-urn", PageNumber, 10, PageSize);
        }
    }
}
