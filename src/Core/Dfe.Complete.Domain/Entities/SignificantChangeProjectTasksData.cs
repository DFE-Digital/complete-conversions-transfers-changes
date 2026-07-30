using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

// Project is the aggregate root. Task data belongs to a project and should be
// created and modified only within that project workflow.
// Note that ConversionTasksData and TransferTasksData have their foreign key on the project
// SignificantChangeProjectTasksData is a child of a SignificantChangeProject instead and stores the project ID
public class SignificantChangeProjectTasksData : IEntity<TaskDataId>
{
    public TaskDataId Id { get; set; }

    public ProjectId ProjectId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual SignificantChangeProject Project { get; private set; }

    private SignificantChangeProjectTasksData()
    {
        Id = default!;
        ProjectId = default!;
        Project = default!;
    }

    internal static SignificantChangeProjectTasksData CreateTask(SignificantChangeProject project)
    {
        ArgumentNullException.ThrowIfNull(project);

        if (project.Id == default) throw new InvalidOperationException("Project ID must be set before creating task data.");

        var now = DateTime.UtcNow;

        return new SignificantChangeProjectTasksData(
            new TaskDataId(Guid.NewGuid()),
            project.Id,
            now,
            now)
        {
            Project = project
        };
    }

    private SignificantChangeProjectTasksData(
        TaskDataId id,
        ProjectId projectId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(projectId);

        if (createdAt == default) throw new ArgumentException("CreatedAt must be provided.", nameof(createdAt));
        if (updatedAt == default) throw new ArgumentException("UpdatedAt must be provided.", nameof(updatedAt));
        if (updatedAt < createdAt) throw new ArgumentException("UpdatedAt cannot be earlier than CreatedAt.", nameof(updatedAt));

        Id = id;
        ProjectId = projectId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Project = default!;
    }
}
