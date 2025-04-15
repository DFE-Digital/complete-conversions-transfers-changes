using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages
{
	public class IndexModel : PageModel
	{
		public async Task<IActionResult> OnGetAsync([FromServices] ISender sender)
		{
			ViewData[TabNavigationModel.ViewDataKey] = new TabNavigationModel(TabNavigationModel.YourProjectsTabName);

			var userTeam = await User.GetUserTeam(sender);

			string route;

			if (userTeam is ProjectTeam.BusinessSupport or ProjectTeam.DataConsumers) route = RouteConstants.ProjectsInProgress;
			else if (userTeam is ProjectTeam.ServiceSupport) route = RouteConstants.ServiceSupportProjects;
			else route = RouteConstants.YourProjectsInProgress;

			return Redirect(route);
		}
	}
}
