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

public abstract class BaseProjectPageModel(ISender sender, ILogger _logger) : PageModel
{
    protected readonly ISender Sender = sender;
    protected ILogger Logger = _logger;


    [BindProperty(SupportsGet = true, Name = "projectId")]

    public string ProjectId { get; set; }

    public ProjectDto Project { get; set; }

    public EstablishmentDto Establishment { get; set; }

    public TrustDto? IncomingTrust { get; set; }

    public TrustDto? OutgoingTrust { get; set; }

    public ProjectTeam CurrentUserTeam { get; set; }

    public async Task UpdateCurrentProject()
    {
        var success = Guid.TryParse(ProjectId, out var guid);

        if (!success)
        {
            Logger.LogWarning("{ProjectId} is not a valid Guid.", ProjectId);
            return;
        }

        var query = new GetProjectByIdQuery(new ProjectId(guid));
        var result = await Sender.Send(query);
        if (!result.IsSuccess || result.Value == null)
        {
            Logger.LogWarning("Project {ProjectId} does not exist.", ProjectId);
            return;
        }

        Project = result.Value;
    }

    protected async Task SetEstablishmentAsync()
    {
        var establishmentQuery = new GetEstablishmentByUrnRequest(Project.Urn.Value.ToString());
        var establishmentResult = await Sender.Send(establishmentQuery);

        if (!establishmentResult.IsSuccess || establishmentResult.Value == null)
        {
            throw new NotFoundException($"Establishment {Project.Urn.Value} does not exist.");
        }

        Establishment = establishmentResult.Value;
    }

    protected async Task SetIncomingTrustAsync()
    {
        if (!Project.FormAMat && Project.IncomingTrustUkprn != null)
        {
            var incomingTrustQuery = new GetTrustByUkprnRequest(Project.IncomingTrustUkprn.Value.ToString());
            var incomingTrustResult = await Sender.Send(incomingTrustQuery);

            if (!incomingTrustResult.IsSuccess || incomingTrustResult.Value == null)
            {
                throw new NotFoundException($"Trust {Project.IncomingTrustUkprn.Value} does not exist.");
            }

            IncomingTrust = incomingTrustResult.Value;
        }
    }

    protected async Task SetOutgoingTrustAsync()
    {
        if (Project.Type == ProjectType.Transfer && Project.OutgoingTrustUkprn != null)
        {
            var outgoingtrustQuery = new GetTrustByUkprnRequest(Project.OutgoingTrustUkprn.Value.ToString());
            var outgoingTrustResult = await Sender.Send(outgoingtrustQuery);

            if (!outgoingTrustResult.IsSuccess || outgoingTrustResult.Value == null)
            {
                throw new NotFoundException($"Trust {Project.OutgoingTrustUkprn.Value} does not exist.");
            }

            OutgoingTrust = outgoingTrustResult.Value;
        }
    }

    protected async Task SetCurrentUserTeamAsync()
    {
        CurrentUserTeam = await User.GetUserTeam(Sender);
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        await UpdateCurrentProject();

        if (Project == null)
            return NotFound();

        await SetEstablishmentAsync();

        await SetIncomingTrustAsync();

        await SetOutgoingTrustAsync();

        await SetCurrentUserTeamAsync();

        return Page();
    }

    public string FormatRouteWithProjectId(string route) => string.Format(route, ProjectId);
}