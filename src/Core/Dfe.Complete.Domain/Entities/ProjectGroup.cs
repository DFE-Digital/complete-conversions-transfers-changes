using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class ProjectGroup : BaseAggregateRoot, IEntity<ProjectGroupId>
{
    public ProjectGroupId Id { get; set; }

    public string? GroupIdentifier { get; set; }

    public Ukprn? TrustUkprn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
