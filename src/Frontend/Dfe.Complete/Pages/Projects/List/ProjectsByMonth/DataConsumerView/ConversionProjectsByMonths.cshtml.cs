using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListProjectsByMonth;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dfe.Complete.Extensions;
using Dfe.Complete.Services;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByMonth
{
    public class ConversionProjectsByMonths(ISender sender, ErrorService errorService) : ProjectsByMonthModel(ConversionsSubNavigation)
    {
        public List<ListProjectsByMonthResultModel> Projects { get; set; } = default!;
        
        [BindProperty(SupportsGet = true, Name = "fromMonth")]
        public int FromMonth { get; set; }
        
        [BindProperty(SupportsGet = true, Name = "fromYear")]
        public int FromYear { get; set; }
        
        [BindProperty(SupportsGet = true, Name = "toMonth")]
        public int ToMonth { get; set; }
        
        [BindProperty(SupportsGet = true, Name = "toYear")]
        public int ToYear { get; set; }
        
        
        [BindProperty(Name = "fromDate")]
        public string FromDate { get; set; }
        
        [BindProperty(Name = "toDate")]
        public string? ToDate { get; set; }
        
        public string DateString { get; set; }

        public async Task OnGet()
        {
            DateOnly.TryParse(string.Format("{0}/{1}", FromMonth, FromYear), out DateOnly fromDate);
            
            DateOnly.TryParse(string.Format("{0}/{1}", ToMonth, ToYear), out DateOnly toDate);
            
            FromDate = ((DateOnly?)fromDate).ToDateMonthYearString();
            ToDate = ((DateOnly?)toDate).ToDateMonthYearString();

            DateString = $"{FromDate} to {ToDate}";
            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
            var listProjectQuery = new ListProjectsByMonthsQuery(fromDate, toDate, ProjectState.Active, ProjectType.Conversion, PageNumber-1, PageSize);
            var response = await sender.Send(listProjectQuery);
            Projects = response.Value?.ToList() ?? [];

            Pagination = new PaginationModel($"/projects/all/by-month/conversions/{FromMonth}/{FromYear}", PageNumber, response.ItemCount, PageSize);
        }

        public async Task OnGetMovePage()
        {
            await OnGet();
        }
        
        public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
        {
            if (!DateTime.TryParse(FromDate, out var fromDate) || !DateTime.TryParse(ToDate, out var toDate))
            {
                errorService.AddError("test", "Invalid date format");
                return Page();
            }

            if (toDate < fromDate)
            {
                errorService.AddError("test", "To date cannot be before From date");
                return Page();
            }
            
            return Redirect($"/projects/all/by-month/conversions/from/{fromDate.Month}/{fromDate.Year}/to/{toDate.Month}/{toDate.Year}");
            
        }
        
        
        public List<DateOnly?> GetMonths()
        {
            var currentYear = DateTime.Now.Year;

            var months = new List<DateOnly?>();

            for (int year = currentYear - 2; year <= currentYear + 2; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    months.Add(DateOnly.FromDateTime(new DateTime(year, month, 1)));
                }
            }

            return months;
        }
    }
}