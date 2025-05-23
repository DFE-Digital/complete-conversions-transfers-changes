using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class LocalAuthority : BaseAggregateRoot, IEntity<LocalAuthorityId>
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

    public static LocalAuthority CreateLocalAuthority(
        LocalAuthorityId id,
        string name,
        string code,
        string address1,
        string? address2,
        string? address3,
        string? addressTown,
        string? addressCounty,
        string addressPostcode,
        DateTime createdAt)
    {
        return new LocalAuthority
        {
            Id = id,
            Name = name,
            Code = code,
            Address1 = address1,
            Address2 = address2,
            Address3 = address3,
            AddressTown = addressTown,
            AddressCounty = addressCounty,
            AddressPostcode = addressPostcode,
            CreatedAt = createdAt
        };
    }
    public void UpdateLocalAuthority( 
       string code,
       string address1,
       string? address2,
       string? address3,
       string? addressTown,
       string? addressCounty,
       string addressPostcode,
       DateTime updatedAt)
    { 
        Code = code;
        Address1 = address1;
        Address2 = address2;
        Address3 = address3;
        AddressTown = addressTown;
        AddressCounty = addressCounty;
        AddressPostcode = addressPostcode;
        UpdatedAt = updatedAt;
    }
}
