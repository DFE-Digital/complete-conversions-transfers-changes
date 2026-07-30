using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class SignificantChangeProject : BaseAggregateRoot, IEntity<ProjectId>
{
    public ProjectId Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public ProjectState State { get; set; }

    public int? PrepareId { get; set; }

    public UserId? AssignedToUserId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public Region? Region { get; set; }

    public Ukprn TrustUkprn { get; set; }

    public string TrustName { get; set; }

    public Urn AcademyUrn { get; set; }

    public DateOnly? SignificantDate { get; set; }

    public ProjectTeam? Team { get; set; }

    public string? SharepointFolderLink { get; set; }

    public string? DecisionConditions { get; set; }

    public virtual User? AssignedToUser { get; set; }

    public virtual SignificantChangeProjectTasksData SignificantTasksData { get; private set; }

    internal SignificantChangeProject()
    {
        Id = default!;
        TrustUkprn = default!;
        TrustName = default!;
        AcademyUrn = default!;
        SignificantTasksData = default!;
    }

    public static SignificantChangeProject CreateProject(
        Ukprn trustUkprn,
        string trustName,
        Urn academyUrn)
    {
        ArgumentNullException.ThrowIfNull(trustUkprn);
        ArgumentNullException.ThrowIfNull(academyUrn);

        if (string.IsNullOrWhiteSpace(trustName))
        {
            throw new ArgumentException("Trust name must be provided", nameof(trustName));
        }

        var now = DateTime.UtcNow;

        var project = new SignificantChangeProject
        {
            Id = new ProjectId(Guid.NewGuid()),
            CreatedAt = now,
            UpdatedAt = now,
            State = ProjectState.Active,
            TrustUkprn = trustUkprn,
            TrustName = trustName,
            AcademyUrn = academyUrn
        };

        project.CreateSignificantChangeProjectTasksData();

        return project;
    }

    public void CreateSignificantChangeProjectTasksData()
    {
        if (Id == default) throw new InvalidOperationException("Project ID must be set before creating task data.");
        if (SignificantTasksData is not null) return;

        SignificantTasksData = SignificantChangeProjectTasksData.CreateTask(this);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignUser(UserId? assignedToUserId)
    {
        AssignedToUserId = assignedToUserId;
        AssignedAt = assignedToUserId is null ? null : DateTime.UtcNow;

        if (assignedToUserId is null)
        {
            AssignedToUser = null;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}