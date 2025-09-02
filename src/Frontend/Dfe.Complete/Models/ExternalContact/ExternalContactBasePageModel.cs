using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Models.ExternalContact;

public abstract class ExternalContactBasePageModel(ISender sender, ILogger logger) : PageModel
{
    protected readonly ISender Sender = sender;
    protected ILogger Logger = logger;

    [BindProperty(SupportsGet = true, Name = "projectId")]
    public string ProjectId { get; set; }

    public ProjectDto? Project { get; set; }

    public TrustDto? IncomingTrust { get; set; }

    public TrustDto? OutgoingTrust { get; set; }

    public EstablishmentDto Establishment { get; set; }


    public virtual async Task<IActionResult> OnGetAsync()
    {  
        await GetCurrentProject();

        if (Project == null)
            return NotFound();

        return Page();
    }

    public async Task GetCurrentProject()
    {
        var success = Guid.TryParse(ProjectId, out var guid);

        if (!success)
        {
            var error = $"{ProjectId} is not a valid Guid.";

            Logger.LogError(error);
            throw new NotFoundException(error);                       
        }

        var query = new GetProjectByIdQuery(new ProjectId(guid));
        var result = await Sender.Send(query);
        if (!result.IsSuccess || result.Value == null)
        {
            var error = $"Project {ProjectId} does not exist.";
            Logger.LogError(error);
            throw new NotFoundException(error);
        }

        Project = result.Value;
    }
}