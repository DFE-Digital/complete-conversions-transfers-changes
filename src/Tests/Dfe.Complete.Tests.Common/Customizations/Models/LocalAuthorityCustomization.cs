using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Tests.Common.Customizations.Models;

public class LocalAuthorityCustomization : ICustomization
{
    public string? LocalAuthorityCode { get; set; }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<LocalAuthorityId>(c =>
            c.FromFactory<Guid>(guid => new LocalAuthorityId(guid)));
            
        fixture
            .Customize<LocalAuthority>(composer => composer
                .With(x => x.Code, LocalAuthorityCode ?? fixture.Create<string>())
                .Do(x => x.Id = fixture.Create<LocalAuthorityId>()));
    }
}