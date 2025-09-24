using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services.Project;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;

namespace Dfe.Complete.Pages.Projects.Completion;

public class CompleteProjectModel(ISender sender, IProjectService projectService, ILogger<CompleteProjectModel> logger)
: BaseProjectPageModel(sender, logger)
{
    private readonly IProjectService projectService = projectService;

    public async override Task<IActionResult> OnGetAsync()
    {
        await UpdateCurrentProject();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await UpdateCurrentProject();
        await GetProjectTaskDataAsync();
        await GetKeyContactForProjectsAsyc();

        var validationErrorUrl = string.Format(RouteConstants.ProjectTaskList, ProjectId) + "?projectCompletionValidation=true";

        if (Project.Type == ProjectType.Transfer)
        {
            var transferTaskList = TransferTaskListViewModel.Create(TransferTaskData, Project, KeyContacts);

            if (transferTaskList != null)
            {
                var validationResult = projectService.GetTransferProjectCompletionResult(Project.SignificantDate, transferTaskList);

                if (validationResult.IsValid)
                {
                    var updateProjectCompletedCommand = new UpdateProjectCompletedCommand(
                        ProjectId: new ProjectId(Guid.Parse(ProjectId))
                    );

                    await Sender.Send(updateProjectCompletedCommand);
                    return Redirect(string.Format(RouteConstants.ProjectCompleteConfirmation, ProjectId));
                }
                else
                {
                    TempData.Put("CompleteProjectValidationMessages", validationResult.ValidationErrors);
                    return Redirect(validationErrorUrl);
                }

            }

            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));

        }
        else
        {
            var conversionTaskList = ConversionTaskListViewModel.Create(ConversionTaskData, Project, KeyContacts);

            var validationResult = projectService.GetConversionProjectCompletionResult(Project.SignificantDate, conversionTaskList);

            if (validationResult.IsValid)
            {
                var updateProjectCompletedCommand = new UpdateProjectCompletedCommand(
                      ProjectId: new ProjectId(Guid.Parse(ProjectId))
                  );

                await Sender.Send(updateProjectCompletedCommand);
                return Redirect(string.Format(RouteConstants.ProjectCompleteConfirmation, ProjectId));
            }
            else
            {
                TempData.Put("CompleteProjectValidationMessages", validationResult.ValidationErrors);
                return Redirect(validationErrorUrl);
            }
        }
    }
}



