using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;

public partial class ArInternalMetadata
{
    public string Key { get; set; } = null!;

    public string? Value { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
