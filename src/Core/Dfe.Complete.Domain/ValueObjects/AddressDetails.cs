namespace Dfe.Complete.Domain.ValueObjects
{
    public record AddressDetails(string Address1, string? Address2, string? Address3, string? AddressTown, string? AddressCounty, string AddressPostcode)
    {
    }
}
