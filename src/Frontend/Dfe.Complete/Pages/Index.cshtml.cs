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

			string route;

			if (User.HasRole("business_support") || User.HasRole("data_consumers")) route = RouteConstants.ProjectsInProgress;
			else if (User.HasRole("service_support")) route = RouteConstants.ServiceSupportProjects;
			else if (User.HasRole("manage_team")) route = RouteConstants.TeamProjectsUnassigned;
			else route = RouteConstants.YourProjectsInProgress;

			return Redirect(route);
		}
	}
}
