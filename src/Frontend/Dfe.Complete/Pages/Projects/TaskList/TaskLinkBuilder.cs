namespace Dfe.Complete.Pages.Projects.TaskList
{
    public class TaskLinkBuilder(string routeConstraints, string projectId)
    {
        public string Build(string task)
        {
            return string.Format(routeConstraints, projectId, task);
        }
    }
}
