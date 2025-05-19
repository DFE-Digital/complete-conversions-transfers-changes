using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using MediatR;

namespace Dfe.Complete.Pages.Projects.TaskList
{
    public class TaskListModel(ISender sender) : BaseProjectPageModel(sender);
}
