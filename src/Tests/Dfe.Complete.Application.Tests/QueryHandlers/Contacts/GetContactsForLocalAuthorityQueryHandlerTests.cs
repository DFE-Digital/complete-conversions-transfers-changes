using System.Linq.Expressions;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Contacts;

public class GetContactsForLocalAuthorityQueryHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnMatchingContacts_WhenLocalAuthorityIdIsValid(
        [Frozen] ICompleteRepository<Contact> mockContactRepo,
        GetContactsForLocalAuthority handler,
        GetContactsForLocalAuthorityQuery query,
        List<Contact> contacts)
    {
        // Arrange
        foreach (var contact in contacts)
        {
            contact.LocalAuthorityId = query.LocalAuthority;
        }

        mockContactRepo
            .FetchAsync(Arg.Any<Expression<Func<Contact, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(contacts);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(contacts.Count, result.Value.Count);
        Assert.All(result.Value, contact =>
            Assert.Equal(query.LocalAuthority, contact.LocalAuthorityId));
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnEmptyList_WhenNoContactsMatch(
        [Frozen] ICompleteRepository<Contact> mockContactRepo,
        GetContactsForLocalAuthority handler,
        GetContactsForLocalAuthorityQuery query)
    {
        // Arrange
        var emptyList = new List<Contact>();

        mockContactRepo
            .FetchAsync(Arg.Any<Expression<Func<Contact, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(emptyList);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException(
        [Frozen] ICompleteRepository<Contact> mockContactRepo,
        GetContactsForLocalAuthority handler,
        GetContactsForLocalAuthorityQuery query)
    {
        // Arrange
        const string expectedErrorMessage = "Database failure";

        mockContactRepo
            .FetchAsync(Arg.Any<Expression<Func<Contact, bool>>>(), Arg.Any<CancellationToken>())
            .Throws(new Exception(expectedErrorMessage));

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedErrorMessage, result.Error);
    }
}
