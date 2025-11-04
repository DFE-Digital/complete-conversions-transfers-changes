using AutoFixture;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Extensions;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class UrnCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new Urn(fixture.CreateInt(100000, 999999)));
        }
    }
}