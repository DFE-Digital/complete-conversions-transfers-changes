using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class SignificantChangeProject : BaseAggregateRoot, IEntity<ProjectId>
{
    public ProjectId Id { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public ProjectState State { get; set; }

    public int? PrepareId { get; set; }

    public UserId? AssignedToUserId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public Region? Region { get; set; }

    public Ukprn TrustUkprn { get; set; } = default!;

    public string TrustName { get; set; } = default!;

    public Urn AcademyUrn { get; set; } = default!;

    public DateOnly? SignificantDate { get; set; }

    public ProjectTeam? Team { get; set; }

    public string? SharepointFolderLink { get; set; }

    public virtual User? AssignedToUser { get; set; }

    internal SignificantChangeProject()
    {
        Id = default!;
        TrustUkprn = default!;
        TrustName = default!;
        AcademyUrn = default!;
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

        return new SignificantChangeProject
        {
            Id = new ProjectId(Guid.NewGuid()),
            CreatedAt = now,
            UpdatedAt = now,
            State = ProjectState.Active,
            TrustUkprn = trustUkprn,
            TrustName = trustName,
            AcademyUrn = academyUrn
        };
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