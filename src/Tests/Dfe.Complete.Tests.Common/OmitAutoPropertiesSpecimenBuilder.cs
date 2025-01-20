using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;

namespace Dfe.Complete.Tests.Common;

public class OmitVirtualPropertiesSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo { GetMethod: { IsVirtual: true, IsFinal: false } })
        {
            return new OmitSpecimen();
        }

        return new NoSpecimen();
    }
}

public class OmitVirtualAutoDataAttribute() : AutoDataAttribute(() =>
{
    var fixture = new Fixture();
    fixture.Customizations.Add(new OmitVirtualPropertiesSpecimenBuilder());
    return fixture;
});