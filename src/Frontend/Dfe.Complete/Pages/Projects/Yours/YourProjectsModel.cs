using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Yours;

public abstract class YourProjectsModel(string currentNavigation) : PageModel
{
    protected TabNavigationModel YourProjectsTabNavigationModel = new(TabNavigationModel.YourProjectsTabName);
    
    public const string InProgressNavigation = "in-progress";
    
    public string CurrentNavigationItem { get; init; } = currentNavigation;

    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public PaginationModel? Pagination { get; set; }

    internal int PageSize = 20;
}