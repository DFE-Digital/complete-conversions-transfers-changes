using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services.Project;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Completion;

    public class CompleteProjectModel(IProjectService projectService, ISender sender, ILogger<CompleteProjectModel> logger)
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



