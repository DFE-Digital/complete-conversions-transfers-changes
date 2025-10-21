using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class AutoFixtureCustomizations : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize(new CompositeCustomization(
                    new AutoNSubstituteCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}
