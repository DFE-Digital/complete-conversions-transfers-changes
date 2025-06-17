using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Extensions;
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

			if (User.HasRole(UserRolesConstants.BusinessSupport) || User.HasRole(UserRolesConstants.DataConsumers)) route = RouteConstants.ProjectsInProgress;
			else if (User.HasRole(UserRolesConstants.ServiceSupport)) route = RouteConstants.ServiceSupportProjectsWithoutAcademyUrn;
			else if (User.HasRole(UserRolesConstants.ManageTeam)) route = RouteConstants.TeamProjectsUnassigned;
			else route = RouteConstants.YourProjectsInProgress;

			return Redirect(route);
		}
	}
}
