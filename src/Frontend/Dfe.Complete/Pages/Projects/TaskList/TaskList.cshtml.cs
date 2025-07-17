using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;

namespace Dfe.Complete.Pages.Projects.TaskList
{
    public class TaskListModel(ISender sender, ILogger<TaskListModel> _logger) : ProjectLayoutModel(sender, _logger, TaskListNavigation);
}
