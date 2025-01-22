using AutoFixture;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class UrnCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<Urn>(() => fixture.CreateUrn());
            //fixture.Customize<Urn>(composer => composer.FromFactory(() => fixture.CreateUrn()));
        }
    }
}
