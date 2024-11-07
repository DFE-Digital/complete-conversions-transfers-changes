using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;

public partial class ProjectGroup
{
    public Guid Id { get; set; }

    public string? GroupIdentifier { get; set; }

    public int? TrustUkprn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
