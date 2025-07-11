using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.ConversionURNs
{
    public class WithAcademyUrnModel(ISender sender) : ServiceSupportModel(ConversionsUrnModel.WithAcademyURNSubNavigation)
    {
        public IEnumerable<ListAllProjectsConvertingQueryResultModel>? Projects { get; set; }
        

        public async Task OnGet()
        {
            var request = new ListAllProjectsConvertingQuery(WithAcademyUrn: true) { Page = PageNumber - 1, Count = PageSize };
            var response = await sender.Send(request);

            Projects = response.Value;
            Pagination = new PaginationModel($"/projects/service-support/with-academy-urn", PageNumber, response.ItemCount, PageSize);
        }
    }
}
