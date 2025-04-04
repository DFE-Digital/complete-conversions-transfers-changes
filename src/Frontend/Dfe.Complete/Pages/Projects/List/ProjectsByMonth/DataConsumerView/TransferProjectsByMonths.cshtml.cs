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
    
        public List<ListProjectsByMonthResultModel> Projects { get; set; } = [];
        
        [BindProperty(SupportsGet = true, Name = "fromMonth")]
        public int FromMonth { get; set; }
        
        [BindProperty(SupportsGet = true, Name = "fromYear")]
        public int FromYear { get; set; }
        
        [BindProperty(SupportsGet = true, Name = "toMonth")]
        public int ToMonth { get; set; }
        
        [BindProperty(SupportsGet = true, Name = "toYear")]
        public int ToYear { get; set; }
        
        [BindProperty(Name = "fromDate")]
        public string FromDate { get; set; } = GetDefaultDate();
        
        [BindProperty(Name = "toDate")]
        public string ToDate { get; set; } = GetDefaultDate();
        
        public string DateRangeDisplay { get; set; } = string.Empty;

        public async Task OnGet()
        {
            var fromDate = ParseDate(FromMonth, FromYear);
            var toDate = ParseDate(ToMonth, ToYear);

            FromDate = fromDate.ToDateMonthYearString();
            ToDate = toDate.ToDateMonthYearString();
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
            
            return RedirectToDateRange(fromDate, toDate);
        }
        
        public List<DateOnly?> GetMonths()
        {
            var currentYear = DateTime.Now.Year;
            var months = new List<DateOnly?>();

            for (int year = currentYear - 2; year <= currentYear + 2; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    months.Add(new DateOnly(year, month, 1));
                }
            }

            return months;
        }

        private DateOnly? ParseDate(int month, int year)
        {
            if (DateOnly.TryParse($"{month}/1/{year}", CultureInfo.InvariantCulture, out var date))
            {
                return date;
            }
            return null;
        }

        private bool TryParseInputDates(out DateTime fromDate, out DateTime toDate)
        {
            bool fromSuccess = DateTime.TryParse(FromDate, out fromDate);
            bool toSuccess = DateTime.TryParse(ToDate, out toDate);
            
            return fromSuccess && toSuccess;
        }

        private void SetErrorNotification(string message)
        {
            TempDataExtensions.SetNotification(
                TempData, 
                NotificationType.Error, 
                "Important", 
                message);
        }

        private void ResetDates()
        {
            var defaultDate = GetDefaultDate();
            
            FromDate = defaultDate;
            ToDate = defaultDate;
        }
        
        private static string GetDefaultDate()
        {
            var today = (DateOnly?)DateOnly.FromDateTime(DateTime.Now);
            return today.ToDateMonthYearString();
        }

        private IActionResult RedirectToDateRange(DateTime fromDate, DateTime toDate)
        {
            var url = string.Format(PathToPage, fromDate.Month, fromDate.Year, toDate.Month, toDate.Year);
            return new RedirectResult(url);
        }
    }
}