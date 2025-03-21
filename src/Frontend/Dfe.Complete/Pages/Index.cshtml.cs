using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages
{
    public class IndexModel : PageModel
    {
		public Task<IActionResult> OnGetAsync()
		{
			ViewData[TabNavigationModel.ViewDataKey] = new TabNavigationModel(TabNavigationModel.YourProjectsTabName);
			return Task.FromResult<IActionResult>(Redirect(RouteConstants.YourProjectsInProgress));
		}
	}
}