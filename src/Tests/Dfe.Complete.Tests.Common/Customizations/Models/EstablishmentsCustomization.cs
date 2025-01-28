using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class EstablishmentsCustomization : ICustomization
    {
        public Urn? Urn { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new UrnSpecimen());

            fixture
                .Customize(new CompositeCustomization(
                   new DateOnlyCustomization()))
                .Customize<GiasEstablishment>(composer => composer);
        }
    }
}
