using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Models.ExternalContact;

public abstract class ExternalContactBasePageModel(IProjectService projectService, ILogger logger) : PageModel
{
    protected readonly IProjectService ProjectService = projectService;
    protected ILogger Logger = logger;

    [BindProperty(SupportsGet = true, Name = "projectId")]
    public string ProjectId { get; set; }

    public ProjectDto? Project { get; set; }

    public TrustDto? IncomingTrust { get; set; }

    public TrustDto? OutgoingTrust { get; set; }

    public async Task GetProject()
    {
       Project = await ProjectService.GetProjectById(ProjectId);
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {  
        await GetProject();

        if (Project == null)
            return NotFound();

        return Page();
    }
}