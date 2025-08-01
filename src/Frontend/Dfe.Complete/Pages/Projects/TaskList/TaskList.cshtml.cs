using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;

namespace Dfe.Complete.Pages.Projects.TaskList
{
    public class TaskListModel(ISender sender, ILogger<TaskListModel> logger) : ProjectLayoutModel(sender, logger, TaskListNavigation);
}
