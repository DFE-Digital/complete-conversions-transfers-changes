using MediatR;

namespace Dfe.Complete.Pages.Projects.TaskList
{
    public class TaskListModel(ISender sender) : Models.BaseProjectTabPageModel(sender, TaskList);
}
