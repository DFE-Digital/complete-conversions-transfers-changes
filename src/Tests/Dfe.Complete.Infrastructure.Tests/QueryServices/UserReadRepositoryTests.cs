using Dfe.Complete.Application.Users.Queries.QueryFilters;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Infrastructure.QueryServices;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dfe.Complete.Infrastructure.Tests.QueryServices
{
    /// <summary>
    /// Tests for UserReadRepository
    /// Validates user query operations
    /// </summary>
    public class UserReadRepositoryTests : IDisposable
    {
        private readonly CompleteContext _context;
        private readonly UserReadRepository _repository;

        public UserReadRepositoryTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<CompleteContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CompleteContext(options);
            _repository = new UserReadRepository(_context);
        }

        [Fact]
        public async Task Users_WithUserIdQuery_ReturnsMatchingUser()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var user = new User
            {
                Id = userId,
                Email = "test@education.gov.uk",
                FirstName = "Test",
                LastName = "User",
                Team = "Regional",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var query = new UserIdQuery(userId).Apply(_repository.Users);
            var result = await query.FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("test@education.gov.uk", result.Email);
        }

        [Fact]
        public async Task Users_WithUserIdQuery_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var nonExistingUserId = new UserId(Guid.NewGuid());

            // Act
            var query = new UserIdQuery(nonExistingUserId).Apply(_repository.Users);
            var result = await query.FirstOrDefaultAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Users_WithActiveTeamLeadersQuery_ReturnsOnlyTeamLeaders()
        {
            // Arrange
            var teamLeader1 = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "leader1@education.gov.uk",
                FirstName = "Leader",
                LastName = "One",
                Team = "Regional",
                ManageTeam = true,
                DeactivatedAt = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var teamLeader2 = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "leader2@education.gov.uk",
                FirstName = "Leader",
                LastName = "Two",
                Team = "Regional",
                ManageTeam = true,
                DeactivatedAt = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var regularUser = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "regular@education.gov.uk",
                FirstName = "Regular",
                LastName = "User",
                Team = "Regional",
                ManageTeam = false, // Not a team leader
                DeactivatedAt = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.AddRange(teamLeader1, teamLeader2, regularUser);
            await _context.SaveChangesAsync();

            // Act
            var query = new ActiveTeamLeadersQuery().Apply(_repository.Users);
            var leaders = await query.ToListAsync();

            // Assert
            Assert.NotNull(leaders);
            Assert.Equal(2, leaders.Count);
            Assert.Contains(leaders, u => u.Id == teamLeader1.Id);
            Assert.Contains(leaders, u => u.Id == teamLeader2.Id);
            Assert.DoesNotContain(leaders, u => u.Id == regularUser.Id);
        }

        [Fact]
        public async Task Users_WithActiveTeamLeadersQuery_NoTeamLeaders_ReturnsEmptyList()
        {
            // Arrange
            var regularUser = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "regular@education.gov.uk",
                FirstName = "Regular",
                LastName = "User",
                Team = "Regional",
                ManageTeam = false, // Not a team leader
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Users.Add(regularUser);
            await _context.SaveChangesAsync();

            // Act
            var query = new ActiveTeamLeadersQuery().Apply(_repository.Users);
            var result = await query.ToListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Users_WithActiveTeamLeadersQuery_IncludesDeactivatedTeamLeaders()
        {
            // Arrange
            var activeLeader = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "active@education.gov.uk",
                FirstName = "Active",
                LastName = "Leader",
                Team = "Regional",
                ManageTeam = true,
                DeactivatedAt = null, // Active
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var deactivatedLeader = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "deactivated@education.gov.uk",
                FirstName = "Deactivated",
                LastName = "Leader",
                Team = "Regional",
                ManageTeam = true,
                DeactivatedAt = DateTime.UtcNow.AddDays(-1), // Deactivated
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.AddRange(activeLeader, deactivatedLeader);
            await _context.SaveChangesAsync();

            // Act
            var query = new ActiveTeamLeadersQuery().Apply(_repository.Users);
            var leaders = await query.ToListAsync();

            // Assert - Both should be returned (no DeactivatedAt filtering)
            Assert.NotNull(leaders);
            Assert.Equal(2, leaders.Count);
            Assert.Contains(leaders, u => u.Id == activeLeader.Id);
            Assert.Contains(leaders, u => u.Id == deactivatedLeader.Id);
        }

        [Fact]
        public void Users_ReturnsQueryableAsNoTracking()
        {
            // Arrange
            var user = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "test@education.gov.uk",
                FirstName = "Test",
                LastName = "User",
                Team = "Regional",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            _context.SaveChangesAsync().Wait();

            // Act
            var users = _repository.Users.ToList();

            // Assert
            Assert.NotEmpty(users);
            Assert.Single(users);
            // Verify AsNoTracking by checking that entities are not tracked
            Assert.Equal(0, _context.ChangeTracker.Entries().Count());
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

