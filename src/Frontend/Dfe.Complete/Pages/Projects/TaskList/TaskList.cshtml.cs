using Dfe.Complete.Application.Projects.Queries.GetConversionTasksData;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData; 
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList
{
    public class TaskListModel(ISender sender, ILogger<TaskListModel> _logger) : ProjectLayoutModel(sender, _logger, TaskListNavigation)
    {
        public TransferTaskListViewModel TransferTaskList { get; set; } = null!;
        public ConversionTaskListViewModel ConversionTaskList { get; set; } = null!;

        public override async Task<IActionResult> OnGetAsync()
        {
            await UpdateCurrentProject();
            await SetEstablishmentAsync();

            if (Project?.Type == ProjectType.Transfer)
            {
                var result = await sender.Send(new GetTransferTasksDataByIdQuery(Project.TasksDataId));
                if (result.IsSuccess && result.Value != null)
                {
                    TransferTaskList = TransferTaskListViewModel.Create(result.Value, Project);
                }
            }
            if (Project?.Type == ProjectType.Conversion)
            {
                var result = await sender.Send(new GetConversionTasksDataByIdQuery(Project.TasksDataId));
                if (result.IsSuccess && result.Value != null)
                {
                    ConversionTaskList = ConversionTaskListViewModel.Create(result.Value, Project);
                }
            }
            return Page();
        } 
    }
}
