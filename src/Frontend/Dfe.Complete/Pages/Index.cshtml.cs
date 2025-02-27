using Dfe.Complete.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages
{
    public class IndexModel : PageModel
    {
		public async Task<IActionResult> OnGetAsync()
		{
			ViewData[TabNavigationModel.ViewDataKey] = new TabNavigationModel(TabNavigationModel.YourProjectsTabName);
			return Page();
		}
	}
}