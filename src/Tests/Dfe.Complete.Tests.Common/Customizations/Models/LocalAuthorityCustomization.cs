using AutoFixture;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class LocalAuthorityCustomization : ICustomization
    {
        public string? LocalAuthorityCode { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture
                .Customize<LocalAuthority>(composer => composer
             .With(x => x.Code, LocalAuthorityCode ?? fixture.Create<string>()));
        }
    }
}
