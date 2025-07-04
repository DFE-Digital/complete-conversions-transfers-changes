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

			if (User.IsInRole(UserRolesConstants.BusinessSupport) || User.IsInRole(UserRolesConstants.DataConsumers)) route = RouteConstants.ProjectsInProgress;
			else if (User.IsInRole(UserRolesConstants.ServiceSupport)) route = RouteConstants.ServiceSupportProjects;
			else if (User.IsInRole(UserRolesConstants.ManageTeam)) route = RouteConstants.TeamProjectsUnassigned;
			else route = RouteConstants.YourProjectsInProgress;

			route = "/projects/865631c0-4542-449a-b63f-78a27279530d/tasks/handover";
			return Redirect(route);
		}
	}
}
