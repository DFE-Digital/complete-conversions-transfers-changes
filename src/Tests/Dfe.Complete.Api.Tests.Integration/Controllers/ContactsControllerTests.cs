using AutoFixture;
using Azure;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WireMock;
using GovUK.Dfe.PersonsApi.Client.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using ContactId = Dfe.Complete.Domain.ValueObjects.ContactId;

namespace Dfe.Complete.Api.Tests.Integration.Controllers;

public class ContactsControllerTests
{

    private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

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
    [Theory]
    [CustomAutoData(
        typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(LocalAuthorityCustomization),
        typeof(ContactCustomization))]
    public async Task ListAllContactsForProjectAndLocalAuthorityAsync_ShouldReturnContacts(
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

        var contacts = fixture.Customize(new ContactCustomization { ProjectId = testProject.Id })
            .CreateMany<Domain.Entities.Contact>(5).Select(contact =>
            {
                contact.Id = new ContactId(Guid.NewGuid());
                return contact;
            }).ToList();

        await dbContext.Contacts.AddRangeAsync(contacts);
        var lastLocalAuth = await dbContext.LocalAuthorities.OrderBy(x=>x.Name).FirstAsync();
        var otherContacts = fixture.Customize(new ContactCustomization { LocalAuthorityId = lastLocalAuth.Id })
            .CreateMany<Domain.Entities.Contact>(1).Select(contact =>
            {
                contact.Id = new ContactId(Guid.NewGuid());
                return contact;
            }).ToList();
        await dbContext.Contacts.AddRangeAsync(otherContacts);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await contactsClient.ListAllContactsForProjectAndLocalAuthorityAsync(testProject.Id.Value, localAuth.Id.Value);

        // Assert
        Assert.NotNull(response);
        var projectContacts = response.Where(x => x.LocalAuthorityId?.Value != localAuth.Id.Value).ToList();
        Assert.Equal(6, response.Count);
        Assert.Equal(contacts.Count, projectContacts.Count);
         
        foreach (var returnedContact in projectContacts)
        {
            Assert.Contains(contacts, c => c.Id.Value == returnedContact.Id?.Value);
        } 
        var otherContact = response.FirstOrDefault(x => x.LocalAuthorityId?.Value == lastLocalAuth.Id.Value);
        Assert.NotNull(otherContact);
    }
    
    [Theory]   
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task GetParliamentMPContactByConstituencyAsync_WhenNoException_Should_Return_OK(
        CustomWebApplicationDbContextFactory<Program> factory,
        IContactsClient contactsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        // Arrange
        var constituencyName = fixture.Create<string>();
        var constituencyMemberContactDto = fixture.Create<ConstituencyMemberContactDto>();
       
        Assert.NotNull(factory.WireMockServer);
        factory.WireMockServer.AddGetWithJsonResponse(string.Format(ConstituenciesClient.GetParliamentMPContact, constituencyName), constituencyMemberContactDto);        

        // Act
        var result = await contactsClient.GetParliamentMPContactByConstituencyAsync(constituencyName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(constituencyMemberContactDto.DisplayNameWithTitle, result.DisplayNameWithTitle);
        Assert.Equal(constituencyMemberContactDto.Email, result.Email);        
    }    

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task GetParliamentMPContactByConstituencyAsync_WhenThrowsException_Should_ReturnError(
        CustomWebApplicationDbContextFactory<Program> factory,
        IContactsClient contactsClient,
        IFixture fixture)
    {

        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        // Arrange        
        var constituencyName = fixture.Create<string>();      

        Assert.NotNull(factory.WireMockServer);
        var requestMatch = Request.Create().WithPath(string.Format(ConstituenciesClient.GetParliamentMPContact, constituencyName))
            .UsingGet();

        factory.WireMockServer.Given(requestMatch).RespondWith(WireMock.ResponseBuilders.Response.Create().WithStatusCode((int)HttpStatusCode.BadRequest).WithHeader("Content-Type", "application/json").WithBody("System.InvalidOperationException: Something failed"));

        var response = await contactsClient.GetParliamentMPContactByConstituencyAsync(constituencyName, default);

        // Assert
        Assert.IsType<Exception>(response);
    }
}