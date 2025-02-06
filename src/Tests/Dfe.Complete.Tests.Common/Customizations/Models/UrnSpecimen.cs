using AutoFixture.Kernel;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class UrnSpecimen : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if(request == null)
            {
                return new NoSpecimen();
            }

            if(typeof(Urn) == request.GetType())
            {
                return new Urn(new Random().Next(10000, 99999));
            }

            return new NoSpecimen();
        }
    }
}
