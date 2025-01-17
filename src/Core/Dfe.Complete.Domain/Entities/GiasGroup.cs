using Dfe.Complete.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace Dfe.Complete.Infrastructure.Models;
// Same here missing implementation of IEntity

public class GiasGroup
{
    public GiasGroupId Id { get; set; }

    public int? Ukprn { get; set; }

    public int? UniqueGroupIdentifier { get; set; }

    public string? GroupIdentifier { get; set; }

    public string? OriginalName { get; set; }

    public string? CompaniesHouseNumber { get; set; }

    public string? AddressStreet { get; set; }

    public string? AddressLocality { get; set; }

    public string? AddressAdditional { get; set; }

    public string? AddressTown { get; set; }

    public string? AddressCounty { get; set; }

    public string? AddressPostcode { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
