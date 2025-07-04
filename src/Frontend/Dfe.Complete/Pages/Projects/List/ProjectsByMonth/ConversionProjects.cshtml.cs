using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dfe.Complete.Extensions;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByMonth
{
    public class ConversionProjectsByMonthModel(ISender sender) : ProjectsByMonthModel(ConversionsSubNavigation)
    {
        public List<ListProjectsByMonthResultModel> Projects { get; set; } = default!;
        
        [BindProperty(SupportsGet = true, Name = "month")]
        public int Month { get; set; }
        
        [BindProperty(SupportsGet = true, Name = "year")]
        public int Year { get; set; }
        
        public string DateString { get; set; }

        public async Task<IActionResult> OnGet()
        {
            DateOnly.TryParse(string.Format("{0}/{1}", Month, Year), out DateOnly date);

            var datetime = (DateTime?)date.ToDateTime(default);
            
            DateString = datetime.ToFullDateMonthYearString();
            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
            var listProjectQuery = new ListProjectsByMonthsQuery(date, null, ProjectState.Active, ProjectType.Conversion, PageNumber-1, PageSize);
            var response = await sender.Send(listProjectQuery);
            Projects = response.Value?.ToList() ?? [];

            Pagination = new PaginationModel($"/projects/all/by-month/conversions/{Month}/{Year}", PageNumber, response.ItemCount, PageSize);

            var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage, Pagination.TotalPages);
            return hasPageFound ?? Page();
        }

        public async Task<IActionResult> OnGetMovePage()
            => await OnGet();   
    }
}