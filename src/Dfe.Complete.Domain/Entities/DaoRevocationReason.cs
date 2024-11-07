using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;

public partial class DaoRevocationReason
{
    public Guid Id { get; set; }

    public Guid? DaoRevocationId { get; set; }

    public string? ReasonType { get; set; }
}
