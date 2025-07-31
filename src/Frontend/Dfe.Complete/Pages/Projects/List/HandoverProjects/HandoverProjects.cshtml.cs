using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.List.HandoverProjects
{
    [Authorize(policy: UserPolicyConstants.CanViewAllProjectsHandover)]
    public class HandoverProjectsModel(ISender sender) : AllProjectsModel(HandoverNavigation)
    {
        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

        [BindProperty]  
        [Display(Name = "Urn")]
        public int? URN { get; set; }

        public bool DisplayMessage { get; set; } = false;
        public async Task<IActionResult> OnGetAsync()
        {
            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
            var orderByClause = new OrderProjectQueryBy(OrderProjectByField.SignificantDate);
            var handoverProjectsQuery = new ListAllProjectsHandoverQuery(Urn: URN.HasValue ? new Urn(URN.Value) : null, OrderBy: orderByClause, Page: PageNumber - 1, Count: PageSize);

            var listResponse = await sender.Send(handoverProjectsQuery);
            Projects = [.. (listResponse.Value ?? [])];

            Pagination = new PaginationModel(RouteConstants.ProjectsHandover, PageNumber, listResponse.ItemCount, PageSize);

            var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
            return hasPageFound ?? Page();
        }

        public string GetHandoverProjectUrl(ProjectId projectId) =>
            string.Format(RouteConstants.ProjectsHandoverCheck, projectId.Value);

        public async Task<IActionResult> OnPostAsync()
        {
            DisplayMessage = URN == null;
            return await OnGetAsync();
        }
    }
}
