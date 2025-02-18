using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.AllProjectsInTrust
{
    public class AllProjectsInTrustViewModel(ISender sender) : AllTrustsWithProjectsPageModel
    {
        [BindProperty(SupportsGet = true, Name = "ukprn")]
        public string Ukprn { get; set; }

        public ListAllProjectsInTrustResultModel? Trust { get; set; } = default!;

        public async Task OnGet()
        {
            var listAllProjectsInTrustQuery = new ListAllProjectsInTrustQuery(Ukprn) { Page = PageNumber - 1, Count = PageSize };


            var trustResponse = await sender.Send(listAllProjectsInTrustQuery);

            Trust = trustResponse.Value;


            var countProjectQuery = new CountAllProjectsQuery(ProjectState.Active, null);
            var countResponse = await sender.Send(countProjectQuery);

            Pagination = new PaginationModel("/projects/all/in-progress/all", PageNumber, countResponse.Value, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}