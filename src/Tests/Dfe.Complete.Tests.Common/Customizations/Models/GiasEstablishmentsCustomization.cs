using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class GiasEstablishmentsCustomization : ICustomization
    {
        public Urn? Urn { get; set; }
        
        public LocalAuthority? LocalAuthority { get; set; }

        public void Customize(IFixture fixture)
        {
            var regionCode = ((char)fixture.Create<Region>()).ToString();

            fixture
                .Customize(new CompositeCustomization(
                    new DateOnlyCustomization()))
                .Customize<GiasEstablishment>(composer =>
                    composer
                        .With(x => x.Urn, () => Urn ?? fixture.Customize(new UrnCustomization()).Create<Urn>())
                        .With(x => x.RegionCode, regionCode)
                        .With(x => x.LocalAuthorityCode, LocalAuthority?.Code ?? fixture.Customize(new LocalAuthorityCustomization()).Create<LocalAuthority>().Code)
                        .With(x => x.LocalAuthorityName, LocalAuthority?.Name ?? fixture.Customize(new LocalAuthorityCustomization()).Create<LocalAuthority>().Code)
                    );
        }
    }
}