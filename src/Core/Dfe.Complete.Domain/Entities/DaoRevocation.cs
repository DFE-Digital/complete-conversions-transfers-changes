using Dfe.Complete.Domain.ValueObjects;

// Please fix the namespace

namespace Dfe.Complete.Infrastructure.Models;

// Same here missing implementation of IEntity

public class DaoRevocation
{
    public DaoRevocationId Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateOnly? DateOfDecision { get; set; }

    public string? DecisionMakersName { get; set; }

    public ProjectId? ProjectId { get; set; }
}
