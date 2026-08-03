using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Tests.Database.Interceptors
{
    public class TimestampAuditInterceptorTests
    {
        [Fact]
        public async Task SaveChangesAsync_WhenSignificantChangeProjectIsModified_ShouldSetUpdatedAt()
        {
            // Arrange
            await using var context = CreateContext();
            var project = SignificantChangeProject.CreateProject(new Ukprn(12345678), "Sample Trust", new Urn(123456));
            var originalUpdatedAt = DateTime.UtcNow.AddDays(-2);
            project.UpdatedAt = originalUpdatedAt;

            context.SignificantChangeProjects.Add(project);
            await context.SaveChangesAsync();

            var beforeUpdate = DateTime.UtcNow;

            // Act
            project.TrustName = "Updated Trust";
            await context.SaveChangesAsync();

            // Assert
            var afterUpdate = DateTime.UtcNow;
            Assert.InRange(project.UpdatedAt, beforeUpdate, afterUpdate);
            Assert.True(project.UpdatedAt > originalUpdatedAt);
        }

        [Fact]
        public async Task SaveChangesAsync_WhenEntityIsAdded_ShouldNotOverwriteUpdatedAt()
        {
            // Arrange
            await using var context = CreateContext();
            var expectedUpdatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var project = SignificantChangeProject.CreateProject(new Ukprn(12345678), "Sample Trust", new Urn(123456));
            project.UpdatedAt = expectedUpdatedAt;

            // Act
            context.SignificantChangeProjects.Add(project);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(expectedUpdatedAt, project.UpdatedAt);
        }

        [Fact]
        public async Task SaveChangesAsync_WhenUserIsModified_ShouldSetUpdatedAt()
        {
            // Arrange
            await using var context = CreateContext();
            var user = User.Create(new UserId(Guid.NewGuid()), "person@education.gov.uk", "First", "Last", null);
            var originalUpdatedAt = DateTime.UtcNow.AddDays(-1);
            user.UpdatedAt = originalUpdatedAt;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var beforeUpdate = DateTime.UtcNow;

            // Act
            user.FirstName = "Updated";
            await context.SaveChangesAsync();

            // Assert
            var afterUpdate = DateTime.UtcNow;
            Assert.InRange(user.UpdatedAt, beforeUpdate, afterUpdate);
            Assert.True(user.UpdatedAt > originalUpdatedAt);
        }

        private static CompleteContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<CompleteContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new CompleteContext(options);
        }
    }
}