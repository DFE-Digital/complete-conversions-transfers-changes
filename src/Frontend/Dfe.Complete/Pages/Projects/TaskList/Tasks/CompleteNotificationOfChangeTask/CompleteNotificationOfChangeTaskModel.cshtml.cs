using Dfe.Complete.Application.Projects.Queries.GetProject;
using MediatR;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.CompleteNotificationOfChangeTask;

public class CompleteNotificationOfChangeTaskModel(ISender sender) : ProjectTaskModel(sender)
{
    // Task-specific properties can be added here
}
