using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Models.ExternalContact;

public abstract class ExternalContactBasePageModel(ISender sender, ILogger logger) : PageModel
{
    [BindProperty(SupportsGet = true, Name = "projectId")]
    public string? ProjectId { get; set; }

    public ProjectDto? Project { get; set; }

    public TrustDto? IncomingTrust { get; set; }

    public TrustDto? OutgoingTrust { get; set; }

    public EstablishmentDto? Establishment { get; set; }

    public async Task GetCurrentProjectAsync()
    {
        if (!Guid.TryParse(ProjectId, out var projectGuid))
        {
            var errorMessage = string.Format(ValidationConstants.InvalidGuid, ProjectId);
            throw new NotFoundException(errorMessage);
        }

        var query = new GetProjectByIdQuery(new ProjectId(projectGuid));
        var result = await sender.Send(query);

        if (!result.IsSuccess || result.Value == null)
        {
            var errorMessage = string.Format(ValidationConstants.ProjectNotFound, ProjectId);
            throw new NotFoundException(errorMessage);
        }

        Project = result.Value;
    }
}