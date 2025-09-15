using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Application.Users.Queries.ListAllUsers;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.User;

public class ListAllUsersWithProjectsHandlerTest
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnSuccess_WhenUsersWithProjectIsFound(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        ListAllUsersWithProjectsHandler handler,
        IFixture fixture)
    {
        // Arrange
        var users = fixture.CreateMany<Domain.Entities.User>(20)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToList();

        for (int i = 0; i < users.Count; i++)
        {
            users[i].ProjectAssignedTos = fixture.CreateMany<Domain.Entities.Project>(i + 1).ToList();
        }

        var expected = users.Select(user => new ListAllUsersWithProjectsResultModel(
            user.Id,
            user.FullName,
            user.Email,
            user.Team.FromDescriptionValue<ProjectTeam>(),
            user.ProjectAssignedTos.Count(project => project.Type == ProjectType.Conversion),
            user.ProjectAssignedTos.Count(project => project.Type == ProjectType.Transfer)
        )).ToList();
        var userQueryable = users.BuildMock();

        mockUserRepository
            .Query()
            .Returns(userQueryable);

        var query = new ListAllUsersWithProjectsQuery(null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        for (int i = 0; i < result.Value!.Count; i++)
        {
            Assert.Equivalent(expected[i], result.Value![i]);
        }
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnSuccess_AndNoUsersWhenThereAreNoAssignedProjects(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        ListAllUsersWithProjectsHandler handler,
        IFixture fixture)
    {
        // Arrange
        var users = fixture.CreateMany<Domain.Entities.User>(20)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToList();

        var userQueryable = users.BuildMock();

        mockUserRepository
            .Query()
            .Returns(userQueryable);

        var query = new ListAllUsersWithProjectsQuery(null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }


    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnSuccess_WhenUsersWithProjectIsFoundAndPaginationWorks(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        ListAllUsersWithProjectsHandler handler,
        IFixture fixture)
    {
        // Arrange
        var users = fixture.CreateMany<Domain.Entities.User>(100)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToList();

        for (int i = 0; i < users.Count; i++)
        {
            users[i].ProjectAssignedTos = fixture.CreateMany<Domain.Entities.Project>(i + 1).ToList();
        }

        var expected = users.Select(user => new ListAllUsersWithProjectsResultModel(
            user.Id,
            user.FullName,
            user.Email,
            user.Team.FromDescriptionValue<ProjectTeam>(),
            user.ProjectAssignedTos.Count(project => project.Type == ProjectType.Conversion),
            user.ProjectAssignedTos.Count(project => project.Type == ProjectType.Transfer)
        )).Skip(10).Take(5).ToList();
        var userQueryable = users.BuildMock();

        mockUserRepository
            .Query()
            .Returns(userQueryable);

        var query = new ListAllUsersWithProjectsQuery(null) { Page = 2, Count = 5 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        for (int i = 0; i < result.Value!.Count; i++)
        {
            Assert.Equivalent(expected[i], result.Value![i]);
        }
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnSuccess_WhenFilterWorksSuccessfully(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        ListAllUsersWithProjectsHandler handler,
        IFixture fixture)
    {
        // Arrange
        var users = fixture.CreateMany<Domain.Entities.User>(10)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToList();

        for (int i = 0; i < users.Count; i++)
        {
            users[i].ProjectAssignedTos = fixture.CreateMany<Domain.Entities.Project>(i + 1).ToList();
        }

        var expected = users.Where(user => user.ProjectAssignedTos.Any(project => project.State == ProjectState.Active)).Select(user => new ListAllUsersWithProjectsResultModel(
            user.Id,
            user.FullName,
            user.Email,
            user.Team.FromDescriptionValue<ProjectTeam>(),
            user.ProjectAssignedTos.Where(project => project.State == ProjectState.Active).Count(project => project.Type == ProjectType.Conversion),
            user.ProjectAssignedTos.Where(project => project.State == ProjectState.Active).Count(project => project.Type == ProjectType.Transfer)
        )).ToList();
        var userQueryable = users.BuildMock();

        mockUserRepository
            .Query()
            .Returns(userQueryable);

        var query = new ListAllUsersWithProjectsQuery(ProjectState.Active);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        for (int i = 0; i < result.Value!.Count; i++)
        {
            Assert.Equivalent(expected[i], result.Value![i]);
        }
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnPaginatedUsersWithProjectCounts(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        ListAllUsersWithProjectsHandler handler,
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

        var query = new ListAllUsersWithProjectsQuery(null, team)
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
        ListAllUsersWithProjectsHandler handler)
    {
        // Arrange
        var usersQueryable = new List<Domain.Entities.User>().AsQueryable().BuildMock();
        mockUserRepository.Query().Returns(usersQueryable);

        var query = new ListAllUsersWithProjectsQuery(null, ProjectTeam.London);

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
        ListAllUsersWithProjectsHandler handler)
    {
        // Arrange
        mockUserRepository.Query().Throws(new Exception("Database error"));

        var query = new ListAllUsersWithProjectsQuery(null, ProjectTeam.London);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Contains("Database error", result.Error);
    }
}