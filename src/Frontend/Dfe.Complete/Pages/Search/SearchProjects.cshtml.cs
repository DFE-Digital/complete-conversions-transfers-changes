using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Application.Projects.Queries.SearchProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.List; 
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Search
{
    public class SearchProjectsModel(ISender sender) : AllProjectsModel(ByUserNavigation)
    {

        [BindProperty(SupportsGet = true)]
        public string Query { get; set; } = string.Empty;

        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;
        public string ErrorMessage { get; set; } = string.Empty;
        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                ErrorMessage = "Please enter a search term";
            }
            else if(Query.Length < 4 )
            {
                ErrorMessage = "Search term too short";
            }
            else if(IsMixedString(Query))
            {
                Projects = [];
            }
            else
            {
                var searchProjectsQuery = new SearchProjectsQuery(ProjectState.Active, Query, PageNumber - 1, 102);

                var searchProjectsResponse = await sender.Send(searchProjectsQuery);
                if(searchProjectsResponse.Value?.Count > 100 )
                {
                    ErrorMessage = "Too many results";
                }
                Projects = searchProjectsResponse.Value ?? []; 
            }
        }
        public static bool IsMixedString(string input)
        {
            bool hasLetter = input.Any(char.IsLetter);
            bool hasDigit = input.Any(char.IsDigit);
            return hasLetter && hasDigit;
        }
    }
}
