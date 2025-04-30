using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Users.Queries.ListAllUsers;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using MockQueryable;
using NSubstitute;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.User;

public class ListAllUsersInTeamWithProjectsHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnPaginatedUsersWithProjectCounts(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        ListAllUsersInTeamWithProjectsHandler handler,
        IFixture fixture)
    {
        // Arrange
        var team = ProjectTeam.London;
        var users = fixture.CreateMany<Domain.Entities.User>(10).ToList();

        foreach (var user in users)
        {
            user.Team = team.ToDescription();
            user.ProjectAssignedTos = fixture.Build<Domain.Entities.Project>()
                .With(p => p.AssignedToId, user.Id)
                .CreateMany(3)
                .ToList();
        }

        var usersQueryable = users.AsQueryable().BuildMock();
        mockUserRepository.Query().Returns(usersQueryable);

        var query = new ListAllUsersInTeamWithProjectsQuery(team)
        {
            Page = 0,
            Count = 10
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(users.Count, result.Value!.Count);

        for (int i = 0; i < users.Count; i++)
        {
            var expectedUser = users[i];
            var expectedConversions = expectedUser.ProjectAssignedTos.Count(p => p.Type == ProjectType.Conversion);
            var expectedTransfers = expectedUser.ProjectAssignedTos.Count(p => p.Type == ProjectType.Transfer);

            var actualUser = result.Value[i];

            Assert.Equal(expectedUser.Id, actualUser.Id);
            Assert.Equal(expectedUser.FullName, actualUser.FullName);
            Assert.Equal(expectedUser.Email, actualUser.Email);
            Assert.Equal(team, actualUser.Team);
            Assert.Equal(expectedConversions, actualUser.ConversionProjectsAssigned);
            Assert.Equal(expectedTransfers, actualUser.TransferProjectsAssigned);
        }
    }

    [Theory]
    [CustomAutoData(typeof(UserCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnEmpty_WhenNoUsersMatchTeam(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        ListAllUsersInTeamWithProjectsHandler handler)
    {
        // Arrange
        var usersQueryable = new List<Domain.Entities.User>().AsQueryable().BuildMock();
        mockUserRepository.Query().Returns(usersQueryable);

        var query = new ListAllUsersInTeamWithProjectsQuery(ProjectTeam.London);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Theory]
    [CustomAutoData(typeof(UserCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        ListAllUsersInTeamWithProjectsHandler handler)
    {
        // Arrange
        mockUserRepository.Query().Throws(new Exception("Database error"));

        var query = new ListAllUsersInTeamWithProjectsQuery(ProjectTeam.London);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Contains("Database error", result.Error);
    }
}
