using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.DateHistory
{
    
    public class ConfirmDateChangeModel(ISender sender) : ProjectLayoutModel(sender, ConversionDateHistoryNavigation)
    {
        public override async Task<IActionResult> OnGet()
        {
            await base.OnGet();
            return Page();
        }
    }
}