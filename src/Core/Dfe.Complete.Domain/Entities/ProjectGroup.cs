using Dfe.Complete.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;
// Same here missing implementation of IEntity

// This is considered an aggregate root as it is not managed by the Project , it should have its own repository/ generic repo

public class ProjectGroup
{
    public ProjectGroupId Id { get; set; }

    public string? GroupIdentifier { get; set; }

    public Ukprn? TrustUkprn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
