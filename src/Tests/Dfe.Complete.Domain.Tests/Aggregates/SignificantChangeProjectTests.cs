using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Tests.Aggregates
{
    public class SignificantChangeProjectTests
    {
        [Fact]
        public void CreateProject_ShouldSetDefaultsAndRequiredFields()
        {
            // Arrange
            var trustUkprn = new Ukprn(12345678);
            var trustName = "Sample Trust";
            var academyUrn = new Urn(123456);
            var beforeCreate = DateTime.UtcNow;

            // Act
            var project = SignificantChangeProject.CreateProject(trustUkprn, trustName, academyUrn);

            // Assert
            var afterCreate = DateTime.UtcNow;
            Assert.NotEqual(Guid.Empty, project.Id.Value);
            Assert.Equal(ProjectState.Active, project.State);
            Assert.Equal(trustUkprn, project.TrustUkprn);
            Assert.Equal(trustName, project.TrustName);
            Assert.Equal(academyUrn, project.AcademyUrn);
            Assert.InRange(project.CreatedAt, beforeCreate, afterCreate);
            Assert.InRange(project.UpdatedAt, beforeCreate, afterCreate);
        }

        [Fact]
        public void CreateProject_ShouldCreateAndLinkSignificantTaskData()
        {
            // Arrange
            var beforeCreate = DateTime.UtcNow;

            // Act
            var project = SignificantChangeProject.CreateProject(
                new Ukprn(12345678),
                "Sample Trust",
                new Urn(123456));

            // Assert
            var afterCreate = DateTime.UtcNow;
            Assert.NotNull(project.SignificantTasksData);
            Assert.NotEqual(Guid.Empty, project.SignificantTasksData.Id.Value);
            Assert.Equal(project.Id, project.SignificantTasksData.ProjectId);
            Assert.Same(project, project.SignificantTasksData.Project);
            Assert.InRange(project.SignificantTasksData.CreatedAt, beforeCreate, afterCreate);
            Assert.InRange(project.SignificantTasksData.UpdatedAt, beforeCreate, afterCreate);
            Assert.Equal(project.SignificantTasksData.CreatedAt, project.SignificantTasksData.UpdatedAt);
        }

        [Fact]
        public void CreateSignificantChangeProjectTasksData_ShouldBeIdempotent()
        {
            // Arrange
            var project = SignificantChangeProject.CreateProject(
                new Ukprn(12345678),
                "Sample Trust",
                new Urn(123456));
            var existingTaskData = project.SignificantTasksData;

            // Act
            project.CreateSignificantChangeProjectTasksData();

            // Assert
            Assert.Same(existingTaskData, project.SignificantTasksData);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateProject_ShouldThrowArgumentException_WhenTrustNameIsMissing(string? trustName)
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(() =>
                SignificantChangeProject.CreateProject(new Ukprn(12345678), trustName!, new Urn(123456)));

            // Assert
            Assert.Equal("trustName", exception.ParamName);
        }

        [Fact]
        public void CreateProject_ShouldThrowArgumentNullException_WhenTrustUkprnIsNull()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
                SignificantChangeProject.CreateProject(null!, "Sample Trust", new Urn(123456)));

            // Assert
            Assert.Equal("trustUkprn", exception.ParamName);
        }

        [Fact]
        public void CreateProject_ShouldThrowArgumentNullException_WhenAcademyUrnIsNull()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
                SignificantChangeProject.CreateProject(new Ukprn(12345678), "Sample Trust", null!));

            // Assert
            Assert.Equal("academyUrn", exception.ParamName);
        }

        [Fact]
        public void AssignUser_ShouldSetAssignedFieldsAndUpdateTimestamp()
        {
            // Arrange
            var project = SignificantChangeProject.CreateProject(new Ukprn(12345678), "Sample Trust", new Urn(123456));
            var previousUpdatedAt = DateTime.UtcNow.AddDays(-5);
            project.UpdatedAt = previousUpdatedAt;
            var assignedUserId = new UserId(Guid.NewGuid());
            var beforeAssign = DateTime.UtcNow;

            // Act
            project.AssignUser(assignedUserId);

            // Assert
            var afterAssign = DateTime.UtcNow;
            Assert.Equal(assignedUserId, project.AssignedToUserId);
            Assert.NotNull(project.AssignedAt);
            Assert.InRange(project.AssignedAt!.Value, beforeAssign, afterAssign);
            Assert.InRange(project.UpdatedAt, beforeAssign, afterAssign);
            Assert.True(project.UpdatedAt > previousUpdatedAt);
        }

        [Fact]
        public void AssignUser_ShouldClearAssignment_WhenAssignedUserIdIsNull()
        {
            // Arrange
            var project = SignificantChangeProject.CreateProject(new Ukprn(12345678), "Sample Trust", new Urn(123456));
            var assignedUserId = new UserId(Guid.NewGuid());
            project.AssignUser(assignedUserId);
            project.AssignedToUser = new User { Id = assignedUserId };
            var beforeUnassign = DateTime.UtcNow;

            // Act
            project.AssignUser(null);

            // Assert
            var afterUnassign = DateTime.UtcNow;
            Assert.Null(project.AssignedToUserId);
            Assert.Null(project.AssignedAt);
            Assert.Null(project.AssignedToUser);
            Assert.InRange(project.UpdatedAt, beforeUnassign, afterUnassign);
        }
    }
}