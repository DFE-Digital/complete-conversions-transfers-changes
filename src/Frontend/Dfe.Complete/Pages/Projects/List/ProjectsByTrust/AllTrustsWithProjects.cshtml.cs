using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.AllProjectsInTrust
{
    public class AllTrustsWithProjectsViewModel(ISender sender) : AllTrustsWithProjectsPageModel
    {
        public List<ListTrustsWithProjectsResultModel> Trusts { get; set; } = default!;

        public async Task OnGet()
        {
            var listProjectByTrustQuery = new ListAllTrustsWithProjectsQuery() { Page = PageNumber - 1, Count = PageSize };

            var listResponse = await sender.Send(listProjectByTrustQuery);
            Trusts = listResponse.Value ?? [];
            
            var countProjectQuery = new CountAllProjectsQuery(ProjectState.Active, null);
            var countResponse = await sender.Send(countProjectQuery);

            Pagination = new PaginationModel("/projects/all/in-progress/all" ,PageNumber, countResponse.Value, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}