using Dfe.Complete.Application.Projects.Queries.TaskData;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList
{
    public class TaskListModel(ISender sender, ILogger<TaskListModel> _logger) : ProjectLayoutModel(sender, _logger, TaskListNavigation)
    {
        public TaskListStatus HandoverWithRegionalDeliveryOfficer { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await UpdateCurrentProject();
            await SetEstablishmentAsync();

            var result = await sender.Send(new GetTaskDataByProjectIdQuery(new ProjectId(Guid.Parse(ProjectId))));
            if (result.IsSuccess && result.Value != null)
            {
                HandoverWithRegionalDeliveryOfficer = TaskListViewModel.HandoverWithRegionalDeliveryOfficerTaskStatus(result.Value);
            }
            return Page();
        }
    }
}
