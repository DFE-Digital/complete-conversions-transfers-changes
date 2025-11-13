using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            ViewData[TabNavigationModel.ViewDataKey] = new TabNavigationModel(TabNavigationModel.YourProjectsTabName);

            string route;

            if (User.IsInRole(UserRolesConstants.DataConsumers)) route = RouteConstants.ProjectsInProgress;
            else if (User.IsInRole(UserRolesConstants.ServiceSupport)) route = RouteConstants.ServiceSupportProjectsWithoutAcademyUrn;
            else if (User.IsInRole(UserRolesConstants.ManageTeam)) route = RouteConstants.TeamProjectsUnassigned;
            else route = RouteConstants.YourProjectsInProgress;

            return Redirect(route);
        }
    }
}
