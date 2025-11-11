using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.AllProjectsInTrust
{
    public class AllTrustsWithProjectsViewModel(ISender sender) : AllProjectsModel(ByTrustNavigation)
    {
        public List<ListTrustsWithProjectsResultModel> Trusts { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            var listProjectByTrustQuery = new ListAllTrustsWithProjectsQuery() { Page = PageNumber - 1, Count = PageSize };

            var listResponse = await sender.Send(listProjectByTrustQuery);
            Trusts = listResponse.Value ?? [];

            Pagination = new PaginationModel("/projects/all/trusts", PageNumber, listResponse.ItemCount, PageSize);

            var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
            return hasPageFound ?? Page();
        }

        public async Task<IActionResult> OnGetMovePage()
        {
            return await OnGet();
        }
    }
}