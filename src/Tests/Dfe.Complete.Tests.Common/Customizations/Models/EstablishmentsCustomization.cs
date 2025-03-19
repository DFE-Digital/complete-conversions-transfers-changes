using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class EstablishmentsCustomization : ICustomization
    {
        public Urn? Urn { get; set; }
        public string? LocalAuthorityCode { get; set; }
        
        public Region? Region { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new UrnSpecimen());

            var region = fixture.Create<Region>();

            fixture
                .Customize(new CompositeCustomization(
                   new DateOnlyCustomization()))
                .Customize<GiasEstablishment>(composer => composer
                    .With(x => x.Urn, Urn ?? fixture.Create<Urn>())
                    .With(x => x.LocalAuthorityCode, LocalAuthorityCode ?? fixture.Create<LocalAuthority>().ToString())
                    .With(x => x.RegionCode, !string.IsNullOrEmpty(Region.GetCharValue()) ? Region.GetCharValue() : region.GetCharValue())
                    .With(x => x.RegionName, !string.IsNullOrEmpty(Region.ToDescription()) ? Region.ToDescription() : region.ToDescription())
                );
        }
    }
}
