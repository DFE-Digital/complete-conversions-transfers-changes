using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Models;

public abstract class BaseProjectPageModel(ISender sender) : PageModel
{
    [BindProperty(SupportsGet = true, Name = "projectId")]

    public string ProjectId { get; set; }

    public ProjectDto Project { get; set; }
    public EstablishmentDto Establishment { get; set; }
    public TrustDto? IncomingTrust { get; set; }
    public TrustDto? OutgoingTrust { get; set; }
    public ProjectTeam CurrentUserTeam { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var success = Guid.TryParse(ProjectId, out var guid);

        if (!success)
        {
            throw new InvalidDataException($"{ProjectId} is not a valid Guid.");
        }

        var query = new GetProjectByIdQuery(new ProjectId(guid));
        var result = await sender.Send(query);
        if (!result.IsSuccess || result.Value == null)
        {
            throw new NotFoundException($"Project {ProjectId} does not exist.");
        }

        Project = result.Value;

        var establishmentQuery = new GetEstablishmentByUrnRequest(Project.Urn.Value.ToString());
        var establishmentResult = await sender.Send(establishmentQuery);

        if (!establishmentResult.IsSuccess || establishmentResult.Value == null)
        {
            throw new NotFoundException($"Establishment {Project.Urn.Value} does not exist.");
        }

        Establishment = establishmentResult.Value;

        if (!Project.FormAMat)
        {
            var incomingTrustQuery = new GetTrustByUkprnRequest(Project.IncomingTrustUkprn.Value.ToString());
            var incomingTrustResult = await sender.Send(incomingTrustQuery);

            if (!incomingTrustResult.IsSuccess || incomingTrustResult.Value == null)
            {
                throw new NotFoundException($"Trust {Project.IncomingTrustUkprn.Value} does not exist.");
            }

            IncomingTrust = incomingTrustResult.Value;
        }

        if (Project.Type == ProjectType.Transfer)
        {
            var outgoingtrustQuery = new GetTrustByUkprnRequest(Project.OutgoingTrustUkprn.Value.ToString());
            var outgoingTrustResult = await sender.Send(outgoingtrustQuery);

            if (!outgoingTrustResult.IsSuccess || outgoingTrustResult.Value == null)
            {
                throw new NotFoundException($"Trust {Project.IncomingTrustUkprn.Value} does not exist.");
            }

            OutgoingTrust = outgoingTrustResult.Value;
        }

        CurrentUserTeam = await User.GetUserTeam(sender);

        return Page();
    }
}