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

			var route = userTeam is ProjectTeam.BusinessSupport or ProjectTeam.DataConsumers
				? RouteConstants.ProjectsInProgress
				: RouteConstants.YourProjectsInProgress;

			return Redirect(route);
		}
	}
}
