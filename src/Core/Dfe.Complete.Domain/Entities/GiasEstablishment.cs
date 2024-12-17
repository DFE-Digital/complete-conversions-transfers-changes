using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Infrastructure.Models;

public class GiasEstablishment
{
    public GiasEstablishmentId Id { get; set; }

    public Urn? Urn { get; set; }

    public Ukprn? Ukprn { get; set; }

    public string? Name { get; set; }

    public string? EstablishmentNumber { get; set; }

    public string? LocalAuthorityName { get; set; }

    public string? LocalAuthorityCode { get; set; }

    public string? RegionName { get; set; }

    public string? RegionCode { get; set; }

    public string? TypeName { get; set; }

    public string? TypeCode { get; set; }

    public int? AgeRangeLower { get; set; }

    public int? AgeRangeUpper { get; set; }

    public string? PhaseName { get; set; }

    public string? PhaseCode { get; set; }

    public string? DioceseName { get; set; }

    public string? DioceseCode { get; set; }

    public string? ParliamentaryConstituencyName { get; set; }

    public string? ParliamentaryConstituencyCode { get; set; }

    public string? AddressStreet { get; set; }

    public string? AddressLocality { get; set; }

    public string? AddressAdditional { get; set; }

    public string? AddressTown { get; set; }

    public string? AddressCounty { get; set; }

    public string? AddressPostcode { get; set; }

    public string? Url { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateOnly? OpenDate { get; set; }

    public string? StatusName { get; set; }
}
