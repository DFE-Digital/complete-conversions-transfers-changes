using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.LocalAuthorities.Models;

public class LocalAuthorityDto
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

    public static LocalAuthorityDto MapLAEntityToDto(LocalAuthority localAuthority) => new()
    {
        Id = localAuthority.Id,
        Name = localAuthority.Name,
        Code = localAuthority.Code,
        Address1 = localAuthority.Address1,
        Address2 = localAuthority.Address2,
        Address3 = localAuthority.Address3,
        AddressTown = localAuthority.AddressTown,
        AddressCounty = localAuthority.AddressCounty,
        AddressPostcode = localAuthority.AddressPostcode
    };
}
