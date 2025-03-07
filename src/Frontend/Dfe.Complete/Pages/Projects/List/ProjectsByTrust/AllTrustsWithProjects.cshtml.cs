using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.AllProjectsInTrust
{
    public class AllTrustsWithProjectsViewModel(ISender sender) : AllProjectsModel(ByTrustNavigation)
    {
        public List<ListTrustsWithProjectsResultModel> Trusts { get; set; } = default!;

        public async Task OnGet()
        {
            var listProjectByTrustQuery = new ListAllTrustsWithProjectsQuery() { Page = PageNumber - 1, Count = PageSize };

            var listResponse = await sender.Send(listProjectByTrustQuery);
            Trusts = listResponse.Value ?? [];
            
            Pagination = new PaginationModel("/projects/all/trusts" ,PageNumber, listResponse.ItemCount, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}