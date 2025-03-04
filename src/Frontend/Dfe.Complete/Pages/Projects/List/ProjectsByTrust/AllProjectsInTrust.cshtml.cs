using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.AllProjectsInTrust
{
    public class AllProjectsInTrustViewModel(ISender sender) : AllProjectsModel(ByTrustNavigation)
    {
        [BindProperty(SupportsGet = true, Name = "ukprn")]
        public string Ukprn { get; set; }
        
        [BindProperty(SupportsGet = true, Name = "reference")]
        public string Reference { get; set; }

        public ListAllProjectsInTrustResultModel? Trust { get; set; } = default!;

        public async Task OnGet()
        {
            bool isFormAMat = !string.IsNullOrEmpty(Reference);
            string identifier = isFormAMat ? Reference : Ukprn;
            
            var listAllProjectsInTrustQuery = new ListAllProjectsInTrustQuery(identifier, isFormAMat) { Page = PageNumber - 1, Count = PageSize };

            var trustResponse = await sender.Send(listAllProjectsInTrustQuery);

            Trust = trustResponse.Value;
            
            var path = isFormAMat ? "reference" : "ukprn";

            Pagination = new PaginationModel($"/projects/all/trusts/{path}/{identifier}", PageNumber, trustResponse.ItemCount, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}