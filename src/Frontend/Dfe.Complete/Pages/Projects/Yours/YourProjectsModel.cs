using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.Yours;

[Authorize(policy: UserPolicyConstants.CanViewYourProjects)]
public abstract class YourProjectsModel(string currentNavigation) : PageModel
{
    protected TabNavigationModel YourProjectsTabNavigationModel = new(TabNavigationModel.YourProjectsTabName);
    
    public const string InProgressNavigation = "in-progress";
    public const string AddedByYouNavigation = "added-by";
    public const string CompletedNavigation = "completed";
    
    public string CurrentNavigationItem { get; init; } = currentNavigation;

    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public PaginationModel? Pagination { get; set; }

    internal int PageSize = 20;
}