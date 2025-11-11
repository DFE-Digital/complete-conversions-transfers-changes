using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Security.Authorization;
using GovUK.Dfe.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Dfe.Complete.Tests.Authorization
{
    public class CustomDatabaseClaimsProviderTests
    {
        private readonly ICompleteRepository<User> _repository;
        private readonly ICustomClaimProvider _provider;
        private readonly IFixture _fixture;

        public CustomDatabaseClaimsProviderTests()
        {
            _repository = Substitute.For<ICompleteRepository<User>>();
            // Use a real MemoryCache instance.
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            _provider = new CustomDatabaseClaimsProvider(_repository, memoryCache);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetClaimsAsync_NoUserId_ReturnsEmpty()
        {
            // Arrange: Create a principal without the object identifier claim.
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            var claims = await _provider.GetClaimsAsync(principal);

            // Assert
            Assert.Empty(claims);
        }

        [Fact]
        public async Task GetClaimsAsync_UserNotFound_ReturnsEmpty()
        {
            // Arrange
            var userId = "123";
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
            });
            var principal = new ClaimsPrincipal(identity);

            // Repository returns null to simulate a user not found.
            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                       .Returns(Task.FromResult<User>(null));

            // Act
            var claims = await _provider.GetClaimsAsync(principal);

            // Assert
            Assert.Empty(claims);
        }

        [Fact]
        public async Task GetClaimsAsync_UserFound_ReturnsExpectedClaims()
        {
            // Arrange
            var userId = "00000000-0000-0000-0000-000000000123";
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
            });
            var principal = new ClaimsPrincipal(identity);

            // Create a user record with specific properties.
            var userRecord = new User
            {
                Id = new UserId(new Guid(userId)),
                ActiveDirectoryUserId = userId,
                Team = "TeamA",
                ManageTeam = true,
                AddNewProject = true,
                AssignToProject = false,
                ManageUserAccounts = true,
                ManageConversionUrns = false,
                ManageLocalAuthorities = true
            };

            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                       .Returns(Task.FromResult(userRecord));

            // Act
            var claims = await _provider.GetClaimsAsync(principal);

            // Assert: Verify the expected claims are present.
            var collection = claims as Claim[] ?? claims.ToArray();

            Assert.NotEmpty(collection);
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "TeamA");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "manage_team");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "add_new_project");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "manage_user_accounts");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "manage_local_authorities");

            // Verify claims that should not be present.
            Assert.DoesNotContain(collection, c => c.Type == ClaimTypes.Role && c.Value == "assign_to_project");
            Assert.DoesNotContain(collection, c => c.Type == ClaimTypes.Role && c.Value == "manage_conversion_urns");
        }

        [Fact]
        public async Task GetClaimsAsync_UserFound_ReturnsAllClaims()
        {
            // Arrange
            var userId = "00000000-0000-0000-0000-000000001234";
            var identity = new ClaimsIdentity(
            [
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
            ]);
            var principal = new ClaimsPrincipal(identity);

            // Create a user record with specific properties.
            var userRecord = new User
            {
                Id = new UserId(new Guid(userId)),
                ActiveDirectoryUserId = userId,
                Team = "london",
                ManageTeam = true,
                AddNewProject = true,
                AssignToProject = true,
                ManageUserAccounts = true,
                ManageConversionUrns = true,
                ManageLocalAuthorities = true
            };

            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                       .Returns(Task.FromResult(userRecord));

            // Act
            var claims = await _provider.GetClaimsAsync(principal);

            // Assert: Verify the expected claims are present.
            var collection = claims as Claim[] ?? claims.ToArray();

            Assert.NotEmpty(collection);
            Assert.Contains(collection, c => c.Type == CustomClaimTypeConstants.UserId && c.Value == "00000000-0000-0000-0000-000000001234");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "london");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "manage_team");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "add_new_project");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "manage_user_accounts");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "regional_delivery_officer");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "manage_local_authorities");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "assign_to_project");
            Assert.Contains(collection, c => c.Type == ClaimTypes.Role && c.Value == "manage_conversion_urns");
        }

        [Fact]
        public async Task GetClaimsAsync_CachesClaims_RepositoryCalledOnce()
        {
            // Arrange
            var userId = "00000000-0000-0000-0000-000000000123";
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
            });
            var principal = new ClaimsPrincipal(identity);

            var userRecord = new User
            {
                Id = new UserId(new Guid(userId)),
                ActiveDirectoryUserId = userId,
                Team = "TeamA",
                ManageTeam = true,
                AddNewProject = true,
                AssignToProject = false,
                ManageUserAccounts = true,
                ManageConversionUrns = false,
                ManageLocalAuthorities = true
            };

            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                       .Returns(Task.FromResult(userRecord));

            // Act: Call GetClaimsAsync twice.
            var claimsFirstCall = await _provider.GetClaimsAsync(principal);
            var claimsSecondCall = await _provider.GetClaimsAsync(principal);

            // Assert: The repository's FindAsync should be called only once due to caching.
            await _repository.Received(1).FindAsync(Arg.Any<Expression<Func<User, bool>>>());

            // Also, verify that the claims returned on both calls are the same.
            var first = claimsFirstCall.Select(c => $"{c.Type}:{c.Value}").OrderBy(x => x);
            var second = claimsSecondCall.Select(c => $"{c.Type}:{c.Value}").OrderBy(x => x);
            Assert.Equal(first, second);
        }
    }
}

