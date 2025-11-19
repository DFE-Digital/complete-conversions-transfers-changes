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

// TODO revisit TODOs
// TODO update manage_team on sign in 
// That should be it for thiS? 

namespace Dfe.Complete.Tests.Authorization
{
    public class CustomDatabaseClaimsProviderTests
    {
        private readonly ICompleteRepository<User> _repository;
        private readonly ICustomClaimProvider _provider;

        public CustomDatabaseClaimsProviderTests()
        {
            _repository = Substitute.For<ICompleteRepository<User>>();
            // Use a real MemoryCache instance.
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            _provider = new CustomDatabaseClaimsProvider(_repository, memoryCache);
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
            var identity = new ClaimsIdentity(
            [
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId)
            ]);
            var principal = new ClaimsPrincipal(identity);

            // Repository returns null to simulate a user not found.
            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                       .Returns(Task.FromResult<User>(null!));

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
            var userEmail = "user@example.com";

            var identity = new ClaimsIdentity([
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId),
                new Claim(CustomClaimTypeConstants.PreferredUsername, userEmail)
            ]);
            var principal = new ClaimsPrincipal(identity);

            // Create a user record with specific properties.
            var userRecord = new User
            {
                Id = new UserId(new Guid(userId)),
                ActiveDirectoryUserId = userId,
                Email = userEmail,
            };

            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                       .Returns(Task.FromResult(userRecord));

            // Act
            var claims = await _provider.GetClaimsAsync(principal);

            // Assert: Verify the expected claims are present.
            var collection = claims as Claim[] ?? claims.ToArray();

            Assert.NotEmpty(collection);
        }

        [Fact]
        public async Task GetClaimsAsync_UserFoundByOid_EmailMismatch_ReturnsEmpty()
        {
            // Arrange
            var userId = "00000000-0000-0000-0000-000000000123";
            var userEmail = "user@example.com";
            var claimEmail = "different@example.com"; // Different email in claims

            var identity = new ClaimsIdentity(new[]
            {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId),
                new Claim(CustomClaimTypeConstants.PreferredUsername, claimEmail)
            });
            var principal = new ClaimsPrincipal(identity);

            var userRecord = new User
            {
                Id = new UserId(new Guid(userId)),
                EntraUserObjectId = userId,
                Email = userEmail, // Different from claim email
                ActiveDirectoryUserId = userId,
                Team = "TeamA"
            };

            // Setup repository to return user on first call (OID lookup)
            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                       .Returns(Task.FromResult(userRecord));

            // Act
            var claims = await _provider.GetClaimsAsync(principal);

            // Assert: Should return empty as no user found
            Assert.Empty(claims);

            // Verify no update was attempted
            await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task GetClaimsAsync_NoOidMatch_NoEmailMatch_ReturnsEmpty()
        {
            // Arrange
            var userId = "00000000-0000-0000-0000-000000000123";
            var userEmail = "user@example.com";

            var identity = new ClaimsIdentity(new[]
            {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId),
                new Claim(CustomClaimTypeConstants.PreferredUsername, userEmail)
            });
            var principal = new ClaimsPrincipal(identity);

            // Setup repository to return null on both OID and email lookups
            _repository.FindAsync(Arg.Any<Expression<Func<User, bool>>>())
                       .Returns(Task.FromResult<User>(null!));

            // Act
            var claims = await _provider.GetClaimsAsync(principal);

            // Assert: Should return empty as no user found
            Assert.Empty(claims);

            // Verify no update was attempted
            await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task GetClaimsAsync_CachesClaims_RepositoryCalledOnce()
        {
            // Arrange
            var userId = "00000000-0000-0000-0000-000000000123";
            var userEmail = "user@example.com";

            var identity = new ClaimsIdentity([
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userId),
                new Claim(CustomClaimTypeConstants.PreferredUsername, userEmail)
            ]);
            var principal = new ClaimsPrincipal(identity);

            var userRecord = new User
            {
                Id = new UserId(new Guid(userId)),
                ActiveDirectoryUserId = userId,
                Email = userEmail,
                Team = "TeamA",
                ManageUserAccounts = true,
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

