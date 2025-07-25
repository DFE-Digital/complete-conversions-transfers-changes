using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.SearchProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Pages.Projects.List;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Dfe.Complete.Pages.Search
{
    [Authorize]
    public class SearchProjectsModel(ISender sender) : AllProjectsModel("")
    {
        [BindProperty(SupportsGet = true, Name = "query")]
        public string Query { get; set; } = string.Empty;

        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

        public string ErrorMessage { get; set; } = string.Empty;

        public int TotalResults { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                ErrorMessage = "Please enter a search term";
            }
            else if (Query.Length < 4)
            {
                ErrorMessage = "Search term too short";
            }
            else if (IsMixedString(Query))
            {
                Projects = [];
            }
            else
            {
                var searchProjectsQuery = new SearchProjectsQuery(Query, [ProjectState.Active, ProjectState.Completed, ProjectState.DaoRevoked])
                {
                    Page = PageNumber - 1,
                    Count = PageSize
                };
                var searchProjectsResponse = await sender.Send(searchProjectsQuery);
                Projects = searchProjectsResponse.Value ?? [];
                TotalResults = searchProjectsResponse.ItemCount; 

                Pagination = new PaginationModel($"/search?query={Query}", PageNumber, TotalResults, PageSize);

                var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
                return hasPageFound ?? Page();
            }
            return Page();
        }
        public static bool IsMixedString(string input)
        {
            bool hasLetter = input.Any(char.IsLetter);
            bool hasDigit = input.Any(char.IsDigit);
            return hasLetter && hasDigit;
        }

        public async Task OnGetMovePage()
        {
            await OnGetAsync();
        }
    }
}
