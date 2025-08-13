using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.ExternalContacts.New;

public class CreateIncomingTrustCeo(ISender sender, ILogger<CreateHeadteacher> logger)
    : ProjectLayoutModel(sender, logger, ExternalContactsNavigation)
{
    public override async Task<IActionResult>  OnGetAsync()
    {
        await base.OnGetAsync();

        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        return RedirectToPage("ExternalContacts", new { ProjectId });
    }
}