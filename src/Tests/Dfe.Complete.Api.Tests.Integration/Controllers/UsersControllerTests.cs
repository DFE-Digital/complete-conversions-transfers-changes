using System.Security.Claims;
using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using GiasEstablishment = Dfe.Complete.Domain.Entities.GiasEstablishment;
using Project = Dfe.Complete.Domain.Entities.Project;

namespace Dfe.Complete.Api.Tests.Integration.Controllers;

public class UsersControllerTests
{
    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateUserAsync_ShouldCreateUser(
        CustomWebApplicationDbContextFactory<Program> factory,
        IUsersClient usersClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();
        var command = new CreateUserCommand()
        {
            FirstName = fixture.Create<string>(),
            LastName = fixture.Create<string>(),
            Email = fixture.Create<string>() + "@education.gov.uk",
            Team = fixture.Create<ProjectTeam>()
        };
        var userId = await usersClient.CreateUserAsync(command, CancellationToken.None);

        Assert.IsType<Guid>(userId);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateUserAsyncShould_UserAlreadyExists_ShouldNotCreateUser(
        CustomWebApplicationDbContextFactory<Program> factory,
        IUsersClient usersClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();

        var existingEmail = fixture.Create<string>() + "@education.gov.uk";

        var existingUser = Domain.Entities.User.Create(
            new Domain.ValueObjects.UserId(Guid.NewGuid()),
            existingEmail,
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<ProjectTeam>().ToString()
        );

        dbContext.Users.Add(existingUser);
        await dbContext.SaveChangesAsync();

        var command = new CreateUserCommand()
        {
            FirstName = fixture.Create<string>(),
            LastName = fixture.Create<string>(),
            Email = existingEmail,
            Team = fixture.Create<ProjectTeam>()
        };

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await usersClient.CreateUserAsync(command, CancellationToken.None));

        Assert.Contains($"User with email '{existingEmail}' already exists", exception.Message);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListUsersWithProjectsAsync_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IUsersClient usersClient,
        IFixture fixture)
    {
        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var testUser = await dbContext.Users.FirstAsync();
        testUser.FirstName = "Nick";
        testUser.LastName = "Warms";
        dbContext.Users.Update(testUser);

        var establishments = fixture.CreateMany<GiasEstablishment>(50).ToList();
        await dbContext.GiasEstablishments.AddRangeAsync(establishments);
        var projects = establishments.Select(establishment =>
        {
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id,
            })
                .Create<Project>();
            project.Urn = establishment.Urn ?? project.Urn;
            return project;
        }).ToList();

        projects.ForEach(p => p.LocalAuthorityId = dbContext.LocalAuthorities.FirstOrDefault().Id);

        await dbContext.Projects.AddRangeAsync(projects);

        await dbContext.SaveChangesAsync();

        // Act
        var results = await usersClient.ListAllUsersWithProjectsAsync(
            null, null, null, 0, 1000);

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        var result = results[0];
        Assert.Equal(testUser.Email, result.Email);
        Assert.Equal(testUser.Id.Value, result.Id.Value);
        Assert.Equal($"{testUser.FirstName} {testUser.LastName}", result.FullName);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task GetUserWithProjectsAsync_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IUsersClient usersClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();
        testUser.FirstName = "Nick";
        testUser.LastName = "Warms";
        dbContext.Users.Update(testUser);

        var localAuthorityId = dbContext.LocalAuthorities.FirstOrDefault().Id;

        var establishments = fixture.CreateMany<GiasEstablishment>(50).ToList();
        await dbContext.GiasEstablishments.AddRangeAsync(establishments);
        var projects = establishments.Select(establishment =>
        {
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id
            })
                .Create<Project>();
            project.Urn = establishment.Urn ?? project.Urn;
            return project;
        }).ToList();

        projects.ForEach(p => p.LocalAuthorityId = localAuthorityId);

        await dbContext.Projects.AddRangeAsync(projects);

        await dbContext.SaveChangesAsync();

        // Act
        var results = await usersClient.GetUserWithProjectsAsync(testUser.Id.Value,
            null, null, 0, 1000);

        // Assert
        Assert.NotNull(results);
        Assert.NotNull(results.ProjectsAssigned);
        Assert.Equal(50, results.ProjectsAssigned!.Count);
        foreach (var result in results.ProjectsAssigned!)
        {
            var project = projects.Find(p => p.Id.Value == result.ProjectId?.Value);
            var establishment = establishments.Find(e => e.Urn?.Value == result.Urn?.Value);

            Assert.NotNull(result.EstablishmentName);
            Assert.Equal(establishment?.Name, result.EstablishmentName);

            Assert.NotNull(result.ProjectId);
            Assert.Equal(project?.Id.Value, result.ProjectId.Value);

            Assert.NotNull(result.Urn);
            Assert.Equal(project?.Urn.Value, result.Urn.Value);
            Assert.Equal(establishment?.Urn?.Value, result.Urn.Value);

            Assert.NotNull(result.ConversionOrTransferDate);
            Assert.Equal(project?.SignificantDate, new DateOnly(result.ConversionOrTransferDate.Value.Year,
                result.ConversionOrTransferDate.Value.Month, result.ConversionOrTransferDate.Value.Day));

            Assert.NotNull(result.State);
            Assert.Equal(project?.State.ToString(), result.State.ToString());

            Assert.NotNull(result.ProjectType);
            Assert.Equal(project?.Type?.ToString(), result.ProjectType.Value.ToString());

            Assert.Equal(project?.FormAMat, result.IsFormAMAT);

            Assert.NotNull(result.AssignedToFullName);
            Assert.Equal($"{project?.AssignedTo?.FirstName} {project?.AssignedTo?.LastName}",
                result.AssignedToFullName);
        }
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task UpdateUserAsync_ShouldUpdateUser(
        CustomWebApplicationDbContextFactory<Program> factory,
        IUsersClient usersClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.UpdateRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();
        
        // Create a user to update
        var existingUser = Domain.Entities.User.Create(
            new Domain.ValueObjects.UserId(Guid.NewGuid()),
            "oldemail@education.gov.uk",
            "Old",
            "User",
            "London"
        );
        
        dbContext.Users.Add(existingUser);
        await dbContext.SaveChangesAsync();

        var updateCommand = new UpdateUserCommand()
        {
            Id = new UserId() { Value = existingUser.Id.Value },
            FirstName = "New",
            LastName = "User",
            Email = "updatedemail@education.gov.uk",
            Team = ProjectTeam.SouthWest
        };

        await usersClient.UpdateUserAsync(updateCommand, CancellationToken.None);

        dbContext.ChangeTracker.Clear();

        // Verify the user was updated
        var updatedUser = await dbContext.Users.FindAsync(existingUser.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("New", updatedUser.FirstName);
        Assert.Equal("User", updatedUser.LastName);
        Assert.Equal("updatedemail@education.gov.uk", updatedUser.Email);
        Assert.Equal("south_west", updatedUser.Team);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task UpdateUserAsync_UserNotFound_ShouldThrowException(
        CustomWebApplicationDbContextFactory<Program> factory,
        IUsersClient usersClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.UpdateRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var nonExistentUserId = new UserId() { Value = Guid.NewGuid() };
        var updateCommand = new UpdateUserCommand()
        {
            Id = nonExistentUserId,
            FirstName = "New",
            LastName = "User",
            Email = "updatedemail@education.gov.uk",
            Team = ProjectTeam.SouthWest
        };

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await usersClient.UpdateUserAsync(updateCommand, CancellationToken.None));

        Assert.Contains($"User with id {nonExistentUserId.Value} not found", exception.Response);
    }


}
