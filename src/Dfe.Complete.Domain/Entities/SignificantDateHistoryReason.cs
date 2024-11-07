using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;

public partial class SignificantDateHistoryReason
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? ReasonType { get; set; }

    public Guid? SignificantDateHistoryId { get; set; }
}
