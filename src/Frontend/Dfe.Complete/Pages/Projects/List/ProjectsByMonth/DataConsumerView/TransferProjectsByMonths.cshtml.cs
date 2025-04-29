using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListProjectsByMonth;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dfe.Complete.Extensions;
using System.Globalization;

namespace Dfe.Complete.Pages.Projects.List.ProjectsByMonth
{
    public class TransferProjectsByMonths(ISender sender) : ProjectsByMonthModel(TransfersSubNavigation)
    {
        private readonly string PathToPage = "/projects/all/by-month/transfers/from/{0}/{1}/to/{2}/{3}";
    
        public async Task OnGet()
        {
            var fromDate = ParseDate(FromMonth, FromYear);
            var toDate = ParseDate(ToMonth, ToYear);

            FromDate = fromDate.ToFullDateMonthYearString();
            ToDate = toDate.ToFullDateMonthYearString();
            DateRangeDisplay = $"{FromDate} to {ToDate}";

            ViewData[TabNavigationModel.ViewDataKey] = AllProjectsTabNavigationModel;
            
            var query = new ListProjectsByMonthsQuery(
                fromDate.Value, 
                toDate, 
                ProjectState.Active, 
                ProjectType.Transfer, 
                PageNumber - 1, 
                PageSize);
                
            var response = await sender.Send(query);
            Projects = response.Value?.ToList() ?? [];
            
            
            var url = string.Format(PathToPage, FromMonth, FromYear, ToMonth, ToYear);

            Pagination = new PaginationModel(
                url, 
                PageNumber, 
                response.ItemCount, 
                PageSize);
        }

        public async Task OnGetMovePage() => await OnGet();
        
        public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
        {
            if (!TryParseInputDates(out var fromDate, out var toDate))
            {
                SetErrorNotification("Invalid date format");
                ResetDates();
                return Page();
            }
            
            if (toDate < fromDate)
            {
                SetErrorNotification("The 'from' date cannot be after the 'to' date");
                ResetDates();
                return Page();
            }
            
            return RedirectToDateRange(PathToPage, fromDate, toDate);
        }
        
        private void ResetDates()
        {
            var defaultDate = GetDefaultDate();
            
            FromDate = defaultDate;
            ToDate = defaultDate;
        }
        
    }
}