using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress
{
    public class MultiAcademyTrustModel(ISender sender) : ProjectsInProgressModel(FormAMatSubNavigation)
    {
        [BindProperty(SupportsGet = true, Name = "reference")]
        public string Reference { get; set; }
        public ListMatResultModel MAT { get; set; } = default!;

        public async Task OnGet()
        {
            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
            var listProjectQuery = new ListEstablishmentsInMatQuery(Reference);

            var response = await sender.Send(listProjectQuery);
            MAT = response.Value;
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}