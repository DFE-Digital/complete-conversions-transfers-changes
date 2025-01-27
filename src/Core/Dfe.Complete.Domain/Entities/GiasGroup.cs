using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class GiasGroup : IEntity<GiasGroupId>
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
