using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList
{
    public class TaskListModel(ISender sender, ILogger<TaskListModel> _logger, IProjectPermissionService projectPermissionService) : ProjectLayoutModel(sender, _logger, projectPermissionService, TaskListNavigation)
    {
        public TransferTaskListViewModel TransferTaskList { get; set; } = null!;
        public ConversionTaskListViewModel ConversionTaskList { get; set; } = null!;

        public string TaskLink(string task) => string.Format(RouteConstants.ProjectTask, ProjectId, task);

        public override async Task<IActionResult> OnGetAsync()
        {
            var baseResult = await base.OnGetAsync();
            if (baseResult is not PageResult) return baseResult;
            await GetProjectTaskDataAsync();
            await GetKeyContactForProjectsAsync();

            TransferTaskList = TransferTaskListViewModel.Create(TransferTaskData, Project, KeyContacts);
            ConversionTaskList = ConversionTaskListViewModel.Create(ConversionTaskData, Project, KeyContacts);
            return Page();
        }
    }
}
