using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;

public partial class KeyContact
{
    public Guid Id { get; set; }

    public Guid? ProjectId { get; set; }

    public Guid? HeadteacherId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid? ChairOfGovernorsId { get; set; }

    public Guid? IncomingTrustCeoId { get; set; }

    public Guid? OutgoingTrustCeoId { get; set; }
}
