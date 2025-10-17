using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Tests.Common.Customizations.Models;

namespace Dfe.Complete.Tests.Common.Customizations.DataAttributes
{
    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute()
            : base(() =>
            {
                return new Fixture().Customize(new AutoFixtureCustomizations());
            })
        { }
    }
}
