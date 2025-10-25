using AutoFixture;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Extensions;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class UkprnCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new Ukprn(fixture.CreateInt(10000000, 19999999)));
        }
    }
}