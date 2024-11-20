using Dfe.Complete.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;

public class DaoRevocationReason
{
    public DaoRevocationReasonId Id { get; set; }

    public DaoRevocationId? DaoRevocationId { get; set; }

    public string? ReasonType { get; set; }
}
