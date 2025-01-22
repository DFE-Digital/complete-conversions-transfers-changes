using AutoFixture;
using Dfe.Complete.Domain.Entities;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class EstablishmentsCustomization : ICustomization
    {
        public string? EstablishmentName { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture
                .Customize(new CompositeCustomization(
                   new UrnCustomization(),
                   new DateOnlyCustomization()))
                .Customize<GiasEstablishment>(composer => composer
             .With(x => x.Name, EstablishmentName ?? fixture.Create<string>()));
        }
    }
}
