using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;

public partial class DaoRevocation
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateOnly? DateOfDecision { get; set; }

    public string? DecisionMakersName { get; set; }

    public Guid? ProjectId { get; set; }
}
