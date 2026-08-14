using Dfe.Complete.Pages.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Models;

public abstract class PaginatedPageModel(string currentNavigation) : PageModel
{
    public string CurrentNavigationItem { get; init; } = currentNavigation;

    [FromQuery(Name = "page")] public int PageNumber { get; set; } = 1;

    public PaginationModel? Pagination { get; set; }

    internal int PageSize = 20;
}