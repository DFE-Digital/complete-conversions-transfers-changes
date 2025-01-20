using Dfe.Complete.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;
// Same here missing implementation of IEntity

public class KeyContact
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
