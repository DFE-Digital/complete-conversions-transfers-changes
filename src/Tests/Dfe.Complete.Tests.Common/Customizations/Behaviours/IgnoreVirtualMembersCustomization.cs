using AutoFixture.Kernel;
using AutoFixture;
using System.Reflection;

namespace Dfe.Complete.Tests.Common.Customizations.Behaviours
{
    public class IgnoreVirtualMembers : ISpecimenBuilder
    {
        public Type ReflectedType { get; }

        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;
            if (pi != null)
            {
                if (ReflectedType == null ||
                    ReflectedType == pi.ReflectedType)
                {
                    if (pi.GetGetMethod().IsVirtual)
                    {
                        return new OmitSpecimen();
                    }
                }
            }

            return new NoSpecimen();
        }
    }

    public class IgnoreVirtualMembersCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new IgnoreVirtualMembers());
        }
    }
}
