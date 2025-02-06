using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class LocalAuthority: BaseAggregateRoot, IEntity<LocalAuthorityId>
{
    public LocalAuthorityId Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Address1 { get; set; } = null!;

    public string? Address2 { get; set; }

    public string? Address3 { get; set; }

    public string? AddressTown { get; set; }

    public string? AddressCounty { get; set; }

    public string AddressPostcode { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
