using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.HandoverProjects
{
    [Authorize(policy: UserPolicyConstants.CanViewAllProjectsHandover)]
    public class HandoverProjectsModel(ISender sender) : AllProjectsModel(HandoverNavigation)
    {
        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;
        [BindProperty(SupportsGet = true, Name = "urn_query")]
        public string? URN { get; set; }
        public string? DisplayMessage { get; set; } = null;

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
            var orderByClause = new OrderProjectQueryBy(OrderProjectByField.SignificantDate);
            var handoverProjectsQuery = new ListAllProjectsHandoverQuery(null, OrderBy: orderByClause, Page: PageNumber - 1, Count: PageSize);

            var listResponse = await sender.Send(handoverProjectsQuery);

            Projects = [.. (listResponse.Value ?? [])];
            Pagination = new PaginationModel(RouteConstants.ProjectsHandover, PageNumber, listResponse.ItemCount, PageSize);
            var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
            return hasPageFound ?? Page(); ;
        }

        public string GetHandoverProjectUrl(ProjectId projectId) =>
            string.Format(RouteConstants.ProjectsHandoverCheck, projectId.Value);


        public async Task<IActionResult> OnGetSearchAsync()
        {
            _ = int.TryParse(URN, out int intUrn);
            var resultPage = await GetProjects(intUrn);
            if (Projects.Count == 0)
            {
                DisplayMessage = $"No project to be handed over with URN {URN} was found";
                resultPage = await GetProjects();
            }
            if (string.IsNullOrWhiteSpace(URN))
            {
                DisplayMessage = "No project to be handed over with URN was found";
            }
            return resultPage;
        }
        private async Task<IActionResult> GetProjects(int? urn = null)
        {
            var orderByClause = new OrderProjectQueryBy(OrderProjectByField.SignificantDate);
            var handoverProjectsQuery = new ListAllProjectsHandoverQuery(Urn: urn.HasValue ? new Urn(urn.Value) : null, OrderBy: orderByClause, Page: PageNumber - 1, Count: PageSize);

            var listResponse = await sender.Send(handoverProjectsQuery);

            Projects = [.. (listResponse.Value ?? [])];
            Pagination = new PaginationModel(RouteConstants.ProjectsHandover, PageNumber, listResponse.ItemCount, PageSize);
            var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
            return hasPageFound ?? Page();
        }
    }
}
