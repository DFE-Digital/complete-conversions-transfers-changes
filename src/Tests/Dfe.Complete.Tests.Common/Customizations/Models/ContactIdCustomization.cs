using AutoFixture;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class ContactIdCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new ContactId(fixture.Create<Guid>()));
        }
    }
}