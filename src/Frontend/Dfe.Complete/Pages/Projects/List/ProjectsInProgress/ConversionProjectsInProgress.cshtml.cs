using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.CountProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List.ProjectsInProgress
{
    public class ConversionProjectsInProgressModel(ISender sender) : AllProjectsViewModel
    {
        public List<ListAllProjectsResultModel> Projects { get; set; } = default!;

        public async Task OnGet()
        {
            //TODO: Review pagination logic
            var listProjectQuery = new ListAllProjectsQuery(ProjectState.Active, ProjectType.Conversion, null, PageNumber-1, PageSize);

            var response = await sender.Send(listProjectQuery);
            Projects = response.Value?.ToList() ?? [];
            
            var countProjectQuery = new CountProjectQuery(ProjectState.Active, ProjectType.Conversion, null);
            var countResponse = await sender.Send(countProjectQuery);

            Pagination = new PaginationModel("/projects/all/in-progress/conversions" ,PageNumber, countResponse.Value, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}