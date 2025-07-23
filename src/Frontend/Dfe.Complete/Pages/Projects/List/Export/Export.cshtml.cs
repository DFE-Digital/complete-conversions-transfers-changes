using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.List.Export;

[Route("projects/all/export")]
[Authorize(policy: UserPolicyConstants.CanViewAllProjectsExports)]
public class ExportRedirectController : Controller
{
    public IActionResult OnGet() => Redirect(string.Format(RouteConstants.Reports));
}
