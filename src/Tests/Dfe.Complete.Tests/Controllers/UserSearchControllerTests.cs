using AutoFixture.Xunit2;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Queries.SearchUsers;
using Dfe.Complete.Controllers;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Dfe.Complete.Tests.Controllers;

public class UserSearchControllerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Index_ReturnsOk_WhenUsersValueIsNull(
        [Frozen] ISender _sender)
    {
        // Arrange
        var query = "test";
        var result = PaginatedResult<List<User>>.Success(null!, 0); // simulate null Value

        _sender.Send(Arg.Is<SearchUsersQuery>(q => q.FilterToAssignableUsers && q.Query == query),
                Arg.Any<CancellationToken>())
            .Returns(result);

        var controller = new UserSearchController(_sender);

        // Act
        var response = await controller.Index(query, "assignable");

        // Assert
        Assert.IsType<OkResult>(response);
    }


    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Index_ReturnsFormattedArray_WhenUsersAreReturned(
        [Frozen] ISender _sender)
    {
        // Arrange
        var users = new List<User>
        {
            new() { FirstName = "Alice", LastName = "Wonder", Email = "alice@example.com" },
            new() { FirstName = "Bob", LastName = "Builder", Email = "bob@example.com" }
        };
        var query = "test";
        var result = PaginatedResult<List<User>>.Success(users, users.Count);

        _sender.Send(Arg.Is<SearchUsersQuery>(q => q.FilterToAssignableUsers && q.Query == query),
                Arg.Any<CancellationToken>())
            .Returns(result);

        var controller = new UserSearchController(_sender);

        // Act
        var response = await controller.Index(query, "assignable");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var array = Assert.IsType<string[][]>(okResult.Value);

        Assert.Equal(2, array.Length);
        Assert.Equal(["Alice", "Wonder", "alice@example.com"], array[0]);
        Assert.Equal(["Bob", "Builder", "bob@example.com"], array[1]);
    }
}