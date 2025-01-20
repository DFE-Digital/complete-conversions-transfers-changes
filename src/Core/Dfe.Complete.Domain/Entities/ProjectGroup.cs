using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

// This is considered an aggregate root as it is not managed by the Project , it should have its own repository/ generic repo

public class ProjectGroup : BaseAggregateRoot, IEntity<ProjectGroupId>
{
    public ProjectGroupId Id { get; set; }

    public string? GroupIdentifier { get; set; }

    public Ukprn? TrustUkprn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
