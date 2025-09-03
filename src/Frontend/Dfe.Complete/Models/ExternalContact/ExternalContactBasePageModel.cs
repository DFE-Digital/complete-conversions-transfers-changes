using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Models.ExternalContact;

public abstract class ExternalContactBasePageModel(ISender sender, ILogger logger) : PageModel
{
    [BindProperty(SupportsGet = true, Name = "projectId")]
    public string ProjectId { get; set; }

    public ProjectDto? Project { get; set; }

    public TrustDto? IncomingTrust { get; set; }

    public TrustDto? OutgoingTrust { get; set; }

    public EstablishmentDto Establishment { get; set; }

    public async Task GetCurrentProject()
    {
        var success = Guid.TryParse(ProjectId, out var guid);

        if (!success)
        {
            var error = $"{ProjectId} is not a valid Guid.";
            logger.LogError(error);
            return;
        }

        var query = new GetProjectByIdQuery(new ProjectId(guid));
        var result = await sender.Send(query);

        if (!result.IsSuccess || result.Value == null)
        {
            var error = $"Project {ProjectId} does not exist.";
            logger.LogError(error);
            return;
        }

        Project = result.Value;
    }
}