using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ExternalContacts.New;

public class NewExternalContact(IProjectService projectService, ILogger<NewExternalContact> logger)
    : ExternalContactBasePageModel(projectService, logger)
{
    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();

        return Page();
    }
}