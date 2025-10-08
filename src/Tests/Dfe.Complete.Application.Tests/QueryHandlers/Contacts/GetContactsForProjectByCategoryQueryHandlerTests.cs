using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Contacts;

public class GetContactsForProjectByCategoryQueryHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation), typeof(ContactCustomization))]
    public async Task Handle_ShouldReturnMatchingContacts_WhenProjectId_ContactCategory_IsValid(
        [Frozen] IContactReadRepository mockContactRepo,
        [Frozen] IMapper mockMapper,
        GetContactsForProjectByCategoryHandler handler,
        GetContactsForProjectByCategoryQuery query,
        Contact contact
        )
    {
        // Arrange
        contact.ProjectId = query.ProjectId;
        contact.Category = query.ContactCategory;   
        contact.Id = new ContactId(Guid.NewGuid());

        // Arrange
        var contactList = new List<Contact> { contact };
        mockContactRepo.Contacts.Returns(contactList.AsQueryable().BuildMock());

        mockMapper.Map<List<ContactDto>>(Arg.Any<List<Contact>>()).Returns([
            new ContactDto
            {
                Id = contact.Id,
                ProjectId = contact.ProjectId,
                Category = contact.Category,
            }]
           );

        // Act
        var result = await handler.Handle(query, default);

        // Assert                
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.All(result.Value, contact =>
            Assert.Equal(query.ProjectId, contact.ProjectId));
        Assert.All(result.Value, contact =>
            Assert.Equal(query.ContactCategory, contact.Category));
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnEmptyList_WhenNoContactsMatch(
        [Frozen] IContactReadRepository mockContactRepo,
        [Frozen] IMapper mockMapper,
        GetContactsForProjectByCategoryHandler handler,
        GetContactsForProjectByCategoryQuery query)
    {
        // Arrange
        var emptyList = new List<Contact>();        
        mockContactRepo.Contacts.Returns(emptyList.AsQueryable().BuildMock());        
        mockMapper.Map<List<ContactDto>>(Arg.Any<List<Contact>>()).Returns([]);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException(
        [Frozen] IContactReadRepository mockContactRepo,
        GetContactsForProjectByCategoryHandler handler,
        GetContactsForProjectByCategoryQuery query)
    {
        // Arrange
        const string expectedErrorMessage = "Unexpected database error";

        mockContactRepo.Contacts.Throws(new Exception(expectedErrorMessage));

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Value);        
        Assert.Equal(expectedErrorMessage, result.Error);
    }
}
