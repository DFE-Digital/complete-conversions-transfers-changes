namespace Dfe.Complete.Domain.Entities
{
	public class TrustDetailsDto
	{
		public string Name { get; set; }

		public string Ukprn { get; set; }

		public string CompaniesHouseNumber { get; set; }

		public GroupContactAddressDto Address { get; set; }

		public GroupType Type { get; set; }

		public string ReferenceNumber { get; set; }
	}

	public class GroupType
	{
		public string Code { get; set; }

		public string Name { get; set; }
	}
}
