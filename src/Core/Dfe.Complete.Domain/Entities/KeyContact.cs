using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class KeyContact : IEntity<KeyContactId>
{
    public KeyContactId Id { get; set; }

    public ProjectId? ProjectId { get; set; }

    public ContactId? HeadteacherId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ContactId? ChairOfGovernorsId { get; set; }

    public ContactId? IncomingTrustCeoId { get; set; }

    public ContactId? OutgoingTrustCeoId { get; set; }
}
