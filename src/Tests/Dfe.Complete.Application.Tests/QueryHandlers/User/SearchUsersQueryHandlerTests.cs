using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Users.Queries.SearchUsers;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryHandlers.User;

public class SearchUsersQueryHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnSuccess_WhenUserIsFound(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        SearchUsersQueryHandler handler,
        Domain.Entities.User user,
        GiasEstablishment establishment,
        IFixture fixture)
    {
        // Arrange
        establishment.Urn ??= new Urn(123456);

        var conversionProject = fixture.Customize(new ProjectCustomization
        {
            Urn = establishment.Urn,
            Type = ProjectType.Conversion,
            AssignedToId = user.Id
        }).Create<Domain.Entities.Project>();
        
        var transferProject = fixture.Customize(new ProjectCustomization
        {
            Urn = establishment.Urn,
            Type = ProjectType.Transfer,
            AssignedToId = user.Id
        }).Create<Domain.Entities.Project>();
        

        var projectList = new List<Domain.Entities.Project>{ conversionProject, transferProject };
        user.ProjectAssignedTos = projectList;
        var userList = new List<Domain.Entities.User>{user};
        
        mockUserRepository
            .FetchAsync(Arg.Any<Expression<Func<Domain.Entities.User, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(userList);

        var query = new SearchUsersQuery(user.FullName);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal(user.Email, result.Value[0].Email);
        Assert.Equal(user.FullName, result.Value[0].FullName);
        Assert.Equivalent(user, result.Value[0]);
    }
}