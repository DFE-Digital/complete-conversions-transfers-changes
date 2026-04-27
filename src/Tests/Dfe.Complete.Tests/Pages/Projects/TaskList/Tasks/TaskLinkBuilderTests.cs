using Dfe.Complete.Pages.Projects.TaskList.Tasks;
using FluentAssertions;

namespace Dfe.Complete.Tests.Pages.Projects.TaskList.Tasks;

public class TaskLinkBuilderTests
{
    [Theory]
    [InlineData("/projects/{0}/tasks/{1}", "123", "my-task", "/projects/123/tasks/my-task")]
    [InlineData("/p/{0}/t/{1}/details", "abc", "task-x", "/p/abc/t/task-x/details")]
    public void Build_ReturnsExpectedLink(string routeConstraints, string projectId, string task, string expected)
    {
        var builder = new TaskLinkBuilder(routeConstraints, projectId);

        var result = builder.Build(task);

        result.Should().Be(expected);
    }
}