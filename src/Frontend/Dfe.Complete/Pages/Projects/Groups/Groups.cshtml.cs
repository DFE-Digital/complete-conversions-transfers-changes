using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Groups
{
    public class ProjectGroupsModel(ISender sender) : BaseProjectsPageModel(String.Empty)
    {
        public List<ListProjectsGroupsModel>? ProjectGroups { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var request = new ListProjectGroupsQuery() { Page = PageNumber - 1, Count = PageSize };
            var response = await sender.Send(request);

            ProjectGroups = response.Value;

            Pagination = new PaginationModel($"/groups", PageNumber, response.ItemCount, PageSize);

            var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
            return hasPageFound ?? Page();
        }
    }
}
