using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListProjectsByMonth;
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

        public async Task OnGet()
        {
            DateOnly.TryParse(string.Format("{0}/{1}", Month, Year), out DateOnly date);

            var datetime = (DateTime?)date.ToDateTime(default);
            
            DateString = datetime.ToDateMonthYearString();
            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
            var listProjectQuery = new ListProjectsByMonthQuery(Month, Year, ProjectState.Active, ProjectType.Conversion, PageNumber-1, PageSize);
            var response = await sender.Send(listProjectQuery);
            Projects = response.Value?.ToList() ?? [];

            Pagination = new PaginationModel($"/projects/all/by-month/conversions/{Month}/{Year}", PageNumber, response.ItemCount, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
    }
}