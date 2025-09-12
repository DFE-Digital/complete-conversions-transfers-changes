using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ContactId = Dfe.Complete.Domain.ValueObjects.ContactId;

namespace Dfe.Complete.Api.Tests.Integration.Controllers;

public class ContactsControllerTests
{
    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task ListAllContactsForProjectAsync_ShouldReturnCorrectContacts(
        CustomWebApplicationDbContextFactory<Program> factory,
        IContactsClient contactsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();
        var localAuth = await dbContext.LocalAuthorities.FirstAsync();
        var establishment = await dbContext.GiasEstablishments.FirstAsync();

        Assert.NotNull(establishment.Urn);

        var testProject = fixture.Customize(new ProjectCustomization
        {
            RegionalDeliveryOfficerId = testUser.Id,
            LocalAuthorityId = localAuth.Id,
            Urn = establishment.Urn
        }).Create<Domain.Entities.Project>();
        
        await dbContext.Projects.AddAsync(testProject);
        await dbContext.SaveChangesAsync();

        var contacts = fixture.Customize(new ContactCustomization { ProjectId = testProject.Id })
            .CreateMany<Domain.Entities.Contact>(5).Select(contact =>
            {
                contact.Id = new ContactId(Guid.NewGuid());
                return contact;
            }).ToList();

        await dbContext.Contacts.AddRangeAsync(contacts);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await contactsClient.ListAllContactsForProjectAsync(testProject.Id.Value);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        Assert.All(result, c =>
        {
            Assert.NotNull(c.ProjectId);
            Assert.Equal(testProject.Id.Value, c.ProjectId.Value);
        });
    }
    
    
    [Theory]
    [CustomAutoData(
        typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(LocalAuthorityCustomization),
        typeof(ContactCustomization))]
    public async Task ListAllContactsForLocalAuthorityAsync_ShouldReturnContacts(
        CustomWebApplicationDbContextFactory<Program> factory,
        IContactsClient contactsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var localAuth = await dbContext.LocalAuthorities.FirstAsync();

        var contacts = fixture.Customize(new ContactCustomization { LocalAuthorityId = localAuth.Id })
            .CreateMany<Domain.Entities.Contact>(5).Select(contact =>
            {
                contact.Id = new ContactId(Guid.NewGuid());
                return contact;
            }).ToList();

        await dbContext.Contacts.AddRangeAsync(contacts);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await contactsClient.ListAllContactsForLocalAuthorityAsync(localAuth.Id.Value);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(5, response.Count);
        Assert.All(response, contact =>
        {
            Assert.NotNull(contact.LocalAuthorityId);
            Assert.Equal(localAuth.Id.Value, contact.LocalAuthorityId.Value);
        });
    }
}