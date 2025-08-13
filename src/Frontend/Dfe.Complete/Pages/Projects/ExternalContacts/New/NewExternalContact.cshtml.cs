using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ExternalContacts.New;

public class NewExternalContact(ISender sender, ILogger<NewExternalContact> logger)
    : ProjectLayoutModel(sender, logger, ExternalContactsNavigation)
{
    public override async Task<IActionResult>  OnGetAsync()
    {
        await base.OnGetAsync();

        return Page();
    }
}