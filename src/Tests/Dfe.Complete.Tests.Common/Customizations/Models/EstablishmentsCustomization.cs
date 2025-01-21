using AutoFixture;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class EstablishmentsCustomization : ICustomization
    {
        public string? EstablishmentName { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<GiasEstablishment>(composer => composer
             .With(x => x.Name, EstablishmentName ?? fixture.Create<string>()));
        }
    }
}
