using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
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
    public EstablishmentDto? Academy { get; set; }
    public TrustDto? IncomingTrust { get; set; }
    public TrustDto? OutgoingTrust { get; set; }
    public ProjectGroupDto? ProjectGroup { get; set; }
    public TransferTaskDataDto? TransferTaskData { get; set; }
    public ProjectTeam CurrentUserTeam { get; set; }

    private async Task SetProject()
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
    }

    private async Task SetEstablishment()
    {
        var establishmentQuery = new GetEstablishmentByUrnRequest(Project.Urn.Value.ToString());
        var establishmentResult = await sender.Send(establishmentQuery);

        if (!establishmentResult.IsSuccess || establishmentResult.Value == null)
        {
            throw new NotFoundException($"Establishment {Project.Urn.Value} does not exist.");
        }

        Establishment = establishmentResult.Value;
    }

    private async Task SetAcademy()
    {
        if (Project.AcademyUrn != null)
        {
            var academyQuery = new GetEstablishmentByUrnRequest(Project.AcademyUrn.Value.ToString());
            var academyResult = await sender.Send(academyQuery);

            if (!academyResult.IsSuccess || academyResult.Value == null)
            {
                throw new NotFoundException($"Academy {Project.AcademyUrn.Value} does not exist.");
            }

            Academy = academyResult.Value;
        }
    }

    private async Task SetIncomingTrust()
    {
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
    }

    private async Task SetOutgoingTrust()
    {
        if (Project.Type == ProjectType.Transfer)
        {
            var outgoingtrustQuery = new GetTrustByUkprnRequest(Project.OutgoingTrustUkprn.Value.ToString());
            var outgoingTrustResult = await sender.Send(outgoingtrustQuery);

            if (!outgoingTrustResult.IsSuccess || outgoingTrustResult.Value == null)
            {
                throw new NotFoundException($"Trust {Project.OutgoingTrustUkprn.Value} does not exist.");
            }

            OutgoingTrust = outgoingTrustResult.Value;
        }
    }

    private async Task SetProjectGroup()
    {
        if (Project.GroupId != null)
        {
            var projectGroupQuery = new GetProjectGroupByIdQuery(Project.GroupId);
            var projectGroup = await sender.Send(projectGroupQuery);
            if (projectGroup.IsSuccess || projectGroup.Value != null)
            {
                ProjectGroup = projectGroup.Value;
            }
        }
    }

    private async Task SetTransferTaskData()
    {
        if (Project.TasksDataId != null)
        {
            var transferTasksDataQuery = new GetTransferTasksDataByIdQuery(Project.TasksDataId);
            var transferTasksData = await sender.Send(transferTasksDataQuery);
            if (transferTasksData.IsSuccess || transferTasksData.Value != null)
            {
                TransferTaskData = transferTasksData.Value;
            }
        }
    }

    public async Task<IActionResult> OnGet()
    {
        await SetProject();

        await SetEstablishment();

        await SetAcademy();

        await SetIncomingTrust();

        await SetOutgoingTrust();

        await SetProjectGroup();

        await SetTransferTaskData();

        CurrentUserTeam = await User.GetUserTeam(sender);

        return Page();
    }
}