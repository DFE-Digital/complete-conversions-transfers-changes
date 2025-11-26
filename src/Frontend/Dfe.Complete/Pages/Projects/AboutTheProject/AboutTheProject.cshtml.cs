using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetGiasEstablishment;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.AboutTheProject;

public class AboutTheProjectModel(ISender sender, ILogger<AboutTheProjectModel> logger, IProjectPermissionService projectPermissionService) : ProjectLayoutModel(sender, logger, projectPermissionService, AboutTheProjectNavigation)
{
    public GiasEstablishmentDto? Academy { get; set; }
    public ProjectGroupDto? ProjectGroup { get; set; }
    public new TransferTaskDataDto? TransferTaskData { get; set; }

    private async Task SetAcademyAsync()
    {
        if (Project.AcademyUrn != null)
        {
            var academyQuery = new GetGiasEstablishmentByUrnQuery(Project.AcademyUrn);
            var academyResult = await Sender.Send(academyQuery);

            if (!academyResult.IsSuccess || academyResult.Value == null)
            {
                throw new NotFoundException($"Academy {Project.AcademyUrn.Value} does not exist.");
            }

            Academy = academyResult.Value;
        }
    }

    private async Task SetProjectGroupAsync()
    {
        if (Project.GroupId != null)
        {
            var projectGroupQuery = new GetProjectGroupByIdQuery(Project.GroupId);
            var projectGroup = await Sender.Send(projectGroupQuery);
            if (projectGroup.IsSuccess || projectGroup.Value != null)
            {
                ProjectGroup = projectGroup.Value;
            }
        }
    }

    private async Task SetTransferTaskDataAsync()
    {
        if (Project.TasksDataId != null)
        {
            var transferTasksDataQuery = new GetTransferTasksDataByIdQuery(Project.TasksDataId);
            var transferTasksData = await Sender.Send(transferTasksDataQuery);
            if (transferTasksData.IsSuccess || transferTasksData.Value != null)
            {
                TransferTaskData = transferTasksData.Value;
            }
        }
    }

    public string GetChangeLinkUrl(string fragment)
    {
        return Project.Type == ProjectType.Conversion
            ? $"{string.Format(RouteConstants.ProjectConversionEdit, ProjectId, fragment)}"
            : $"{string.Format(RouteConstants.ProjectTransferEdit, ProjectId, fragment)}";
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        await SetAcademyAsync();

        await SetProjectGroupAsync();

        await SetTransferTaskDataAsync();

        return Page();
    }
}
