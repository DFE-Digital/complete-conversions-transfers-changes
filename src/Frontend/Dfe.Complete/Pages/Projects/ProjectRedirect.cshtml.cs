using Dfe.Complete.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects;

[Route("projects/{projectId:guid}")]
public class RedirectController : Controller
{
    [BindProperty(SupportsGet = true, Name = "projectId")]
    public Guid ProjectId { get; set; }

    public IActionResult OnGet()
    {
        return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
    }
}
