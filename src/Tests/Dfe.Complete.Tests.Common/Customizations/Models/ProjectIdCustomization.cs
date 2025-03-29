using AutoFixture;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Extensions;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class ProjectIdCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new ProjectId(fixture.Create<Guid>()));
        }
    }
}