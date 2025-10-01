using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class DaoRevocation : BaseAggregateRoot, IEntity<DaoRevocationId>
{
    public DaoRevocationId Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateOnly? DateOfDecision { get; set; }

    public string? DecisionMakersName { get; set; }

    public ProjectId? ProjectId { get; set; }
}
