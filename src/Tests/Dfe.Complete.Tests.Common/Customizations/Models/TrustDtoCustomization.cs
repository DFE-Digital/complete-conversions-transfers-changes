using AutoFixture;
using Dfe.AcademiesApi.Client.Contracts;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class TrustDtoCustomization : ICustomization
    {
        public string? Ukprn { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<AddressDto>(composer => composer
             .With(x => x.Street, fixture.Create<string>())
             .With(x => x.Locality, fixture.Create<string>())
             .With(x => x.Additional, fixture.Create<string>())
             .With(x => x.Town, fixture.Create<string>())
             .With(x => x.County, fixture.Create<string>())
             .With(x => x.Postcode, fixture.Create<string>())
            );

            fixture.Customize<TrustDto>(composer => composer
             .With(x => x.Ukprn, Ukprn ?? fixture.Create<int>().ToString()));

        }
    }
}
