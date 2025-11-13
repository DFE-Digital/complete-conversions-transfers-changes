using AutoFixture.Xunit2;
using Xunit;

namespace Dfe.Complete.Tests.Common.Customizations.DataAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class InlineAutoNSubstituteDataAttribute : CompositeDataAttribute
    {
        public InlineAutoNSubstituteDataAttribute(params object[] values)
            : base([
                new InlineDataAttribute(values),
                new AutoNSubstituteDataAttribute()
            ])
        {
        }
    }
}
