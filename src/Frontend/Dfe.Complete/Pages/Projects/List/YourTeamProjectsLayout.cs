// using Dfe.Complete.Application.Projects.Models;
// using Dfe.Complete.Constants;
// using Dfe.Complete.Domain.Entities;
// using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List;

public abstract class YourTeamProjectsModel(string currentNavigation) : PageModel
{
    protected TabNavigationModel YourTeamProjectsTabNavigationModel = new(TabNavigationModel.YourTeamProjectsTabName);

    public const string InProgressNavigation = "in-progress";

    public string CurrentNavigationItem { get; init; } = currentNavigation;

    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public PaginationModel? Pagination { get; set; }

    internal int PageSize = 20;
}