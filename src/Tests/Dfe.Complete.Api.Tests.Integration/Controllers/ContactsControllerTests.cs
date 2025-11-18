using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using GovUK.Dfe.PersonsApi.Client.Contracts;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
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
        var lastLocalAuth = await dbContext.LocalAuthorities.OrderBy(x => x.Name).FirstAsync();
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
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        // Arrange
        var constituencyName = fixture.Create<string>();
        var constituencyMemberContactDto = fixture.Create<Complete.Client.Contracts.ConstituencyMemberContactDto>();
        var baseAddressUrl = "https://mock/api/test";
        var baseAddress = new Uri(baseAddressUrl);

        var response = new HttpResponseMessage(HttpStatusCode.OK);

        response.Content = new StringContent(JsonSerializer.Serialize(constituencyMemberContactDto, jsonSerializerOptions), System.Text.Encoding.UTF8, "application/json");

        Mock<HttpMessageHandler> httpMessageHandlerMock = new();

        httpMessageHandlerMock.Protected()
          .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(response);

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        httpClient.BaseAddress = baseAddress;

        var contactsClient = new ContactsClient(baseAddressUrl, httpClient);

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
        IFixture fixture)
    {

        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        // Arrange        
        var constituencyName = fixture.Create<string>();
        var baseAddressUrl = "https://mock/api/test";
        var baseAddress = new Uri(baseAddressUrl);

        Mock<HttpMessageHandler> httpMessageHandlerMock = new();

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new PersonsApiException("An error occurred with the Persons API client", (int)HttpStatusCode.BadRequest, null, null, null));

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        httpClient.BaseAddress = baseAddress;

        var contactsClient = new ContactsClient(baseAddressUrl, httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<PersonsApiException>(async () =>
        {
            await contactsClient.GetParliamentMPContactByConstituencyAsync(constituencyName, default);
        });
    }

    [Theory]
    [CustomAutoData(
        typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(LocalAuthorityCustomization),
        typeof(ContactCustomization))]
    public async Task GetExternalContactById_Should_Return_The_Contact(
        CustomWebApplicationDbContextFactory<Program> factory,
        IContactsClient contactsClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ApiRoles.ReadRole}
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();
        var expected = fixture.Create<Domain.Entities.Contact>();
        expected.Type = Domain.Enums.ContactType.Project;
        expected.Category = Domain.Enums.ContactCategory.Other;
        dbContext.Contacts.Add(expected);

        await dbContext.SaveChangesAsync();

        var actual = await contactsClient.GetExternalContactAsync(expected.Id.Value);

        Assert.Equivalent(expected.Id, actual.Id);
        Assert.Equivalent(expected.ProjectId, actual.ProjectId);
        Assert.Equivalent(expected.Name, actual.Name);        
        Assert.Equivalent(expected.Title, actual.Title);
        Assert.Equivalent(expected.Email, actual.Email);        
        Assert.Equivalent(expected.Phone, actual.Phone);       
        Assert.Equivalent(expected.OrganisationName, actual.OrganisationName);
        Assert.Equal(expected.LocalAuthorityId?.Value, actual.LocalAuthorityId?.Value);
        Assert.Equal(ContactCategory.Other, actual.Category);
        Assert.Equal(ContactType.Project, actual.Type);
        Assert.Equivalent(expected.EstablishmentUrn, actual.EstablishmentUrn);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateExternalContact_Async_ShouldCreateContact(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateExternalContactCommand createExternalContactCommand,
        IContactsClient contactsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var contactId = await contactsClient.CreateExternalContactAsync(createExternalContactCommand, CancellationToken.None);
        Assert.IsType<Complete.Client.Contracts.ContactId>(contactId);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(ContactCustomization),
        typeof(OmitCircularReferenceCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task UpdateExternalContactAsync_ShouldUpdateContact(
        CustomWebApplicationDbContextFactory<Program> factory,
        UpdateExternalContactCommand updateExternalContactCommand,
        IContactsClient contactsClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.UpdateRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();

        // Create a contact to update
        var existingContact = fixture.Create<Domain.Entities.Contact>();

        dbContext.Contacts.Add(existingContact);
        await dbContext.SaveChangesAsync();

        updateExternalContactCommand.ContactDto!.Id = new Complete.Client.Contracts.ContactId() { Value = existingContact.Id.Value }; 

        await contactsClient.UpdateExternaContactAsync(updateExternalContactCommand, CancellationToken.None);

        dbContext.ChangeTracker.Clear();

        // Verify the contact was updated
        var updatedContact = await dbContext.Contacts.FindAsync(existingContact.Id);
        var updateContactDto = updateExternalContactCommand.ContactDto;
        Assert.NotNull(updatedContact);
        Assert.Equal(updateContactDto?.Name, updatedContact.Name);
        Assert.Equal(updateContactDto?.Title, updatedContact.Title);
        Assert.Equal(updateContactDto?.Email, updatedContact.Email);
        Assert.Equal(updateContactDto?.OrganisationName, updatedContact.OrganisationName);
        Assert.Equal(updateContactDto?.Phone, updatedContact.Phone);        
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(ContactCustomization),
        typeof(OmitCircularReferenceCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task UpdateUserAsync_ContactNotFound_ShouldThrowException(
        CustomWebApplicationDbContextFactory<Program> factory,
        UpdateExternalContactCommand updateExternalContactCommand,
        IContactsClient contactsClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.UpdateRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();

        // Create a contact to update
        var existingContact = fixture.Create<Domain.Entities.Contact>();

        dbContext.Contacts.Add(existingContact);
        await dbContext.SaveChangesAsync();

        var nonExistendContactId = new Complete.Client.Contracts.ContactId() { Value = Guid.NewGuid() };
        updateExternalContactCommand.ContactDto!.Id = nonExistendContactId;        

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await contactsClient.UpdateExternaContactAsync(updateExternalContactCommand, CancellationToken.None));
        
        Assert.Contains($"External contact with Id {nonExistendContactId.Value} not found", exception.Response);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(ContactCustomization),
        typeof(OmitCircularReferenceCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task DeleteExternalContactAsync_ShouldDeleteContact(
        CustomWebApplicationDbContextFactory<Program> factory,        
        IContactsClient contactsClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.UpdateRole, ApiRoles.DeleteRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();

        // Create a contact
        var existingContact = fixture.Create<Domain.Entities.Contact>();

        dbContext.Contacts.Add(existingContact);
        await dbContext.SaveChangesAsync();

        await contactsClient.DeleteExternalContactAsync(new Complete.Client.Contracts.ContactId() { Value = existingContact.Id.Value }, CancellationToken.None);

        dbContext.ChangeTracker.Clear();

        // Verify the contact was deleted
        var contact = await dbContext.Contacts.SingleOrDefaultAsync(x => x.Id == existingContact.Id);
        Assert.Null(contact);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(ContactCustomization),
        typeof(OmitCircularReferenceCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task DeleteExternalContactAsync_ContactNotFound_ShouldThrowException(
        CustomWebApplicationDbContextFactory<Program> factory,        
        IContactsClient contactsClient,
        IFixture fixture)
    {

        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.UpdateRole, ApiRoles.DeleteRole }
           .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();

        // Create a contact
        var existingContact = fixture.Create<Domain.Entities.Contact>();

        dbContext.Contacts.Add(existingContact);
        await dbContext.SaveChangesAsync();

        var nonExistendContactId = new Complete.Client.Contracts.ContactId() { Value = Guid.NewGuid() };

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
           await contactsClient.DeleteExternalContactAsync(new Complete.Client.Contracts.ContactId() { Value = nonExistendContactId.Value }, CancellationToken.None));

        Assert.Contains($"External contact with Id {nonExistendContactId.Value} not found", exception.Response);
    }
}