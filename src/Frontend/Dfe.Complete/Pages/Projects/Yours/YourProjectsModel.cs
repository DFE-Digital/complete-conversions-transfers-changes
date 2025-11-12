using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Models;
using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Pages.Projects.Yours;

[Authorize(policy: UserPolicyConstants.CanViewYourProjects)]
public abstract class YourProjectsModel(string currentNavigation) : BaseProjectsPageModel(currentNavigation)
{
    protected TabNavigationModel YourProjectsTabNavigationModel = new(TabNavigationModel.YourProjectsTabName);

    public const string InProgressNavigation = "in-progress";
    public const string AddedByYouNavigation = "added-by";
    public const string CompletedNavigation = "completed";
}