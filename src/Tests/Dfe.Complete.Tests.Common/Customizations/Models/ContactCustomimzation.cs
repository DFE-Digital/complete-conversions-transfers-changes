using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Extensions;

namespace Dfe.Complete.Tests.Common.Customizations.Models;

public class ContactCustomization : ICustomization
{
    public ProjectId? ProjectId { get; set; }
    public LocalAuthorityId? LocalAuthorityId { get; set; }
    
    public int? Urn { get; set; }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<ContactId>(c =>
            c.FromFactory<Guid>(guid => new ContactId(guid)));

        fixture.Customize<Contact>(composer => composer
                .With(c => c.Name, fixture.Create<string>())
                .With(c => c.Title, fixture.Create<string>())
                .With(c => c.Email, fixture.Create<string>() + "@test.com")
                .With(c => c.Phone, fixture.Create<string>().Substring(0, 10))
                .With(c => c.CreatedAt, DateTime.UtcNow.AddDays(-10))
                .With(c => c.UpdatedAt, DateTime.UtcNow)
                .With(c => c.Category, fixture.Create<ContactCategory>())
                .With(c => c.OrganisationName, fixture.Create<string>())
                .With(c => c.Type, fixture.Create<string>())
                .With(c => c.ProjectId, ProjectId)
                .With(c => c.LocalAuthorityId, LocalAuthorityId ?? fixture.Create<LocalAuthorityId>())
                .With(c => c.EstablishmentUrn, Urn ?? fixture.CreateInt(10000, 99999))
                .Without(c => c.Project) // Avoid circular dependencies
        );
    }
}