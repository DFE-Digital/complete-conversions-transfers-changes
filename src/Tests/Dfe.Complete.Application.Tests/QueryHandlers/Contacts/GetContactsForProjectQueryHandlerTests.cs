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

public class GetContactsForProjectQueryHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnMatchingContacts_WhenProjectIdIsValid(
        [Frozen] ICompleteRepository<Contact> mockContactRepo,
        GetContactsForProject handler,
        GetContactsForProjectQuery query,
        List<Contact> contacts)
    {
        // Arrange
        foreach (var contact in contacts)
        {
            contact.ProjectId = query.ProjectId;
        }

        mockContactRepo
            .FetchAsync(Arg.Any<Expression<Func<Contact, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(contacts);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.Equal(contacts.Count, result.Value.Count);
        Assert.All(result.Value, contact =>
            Assert.Equal(query.ProjectId, contact.ProjectId));
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnEmptyList_WhenNoContactsMatch(
        [Frozen] ICompleteRepository<Contact> mockContactRepo,
        GetContactsForProject handler,
        GetContactsForProjectQuery query)
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
        GetContactsForProject handler,
        GetContactsForProjectQuery query)
    {
        // Arrange
        const string expectedErrorMessage = "Unexpected database error";

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
