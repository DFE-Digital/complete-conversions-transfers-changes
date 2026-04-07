using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Validation;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Xunit;

namespace Dfe.Complete.Application.Tests.Validators
{
    /// <summary>
    /// Tests for SignificantDateValidator
    /// Validates business rules for project significant dates
    /// </summary>
    public class SignificantDateValidatorTests
    {
        private readonly SignificantDateValidator _validator;

        public SignificantDateValidatorTests()
        {
            _validator = new SignificantDateValidator();
        }

        #region ValidateSignificantDateInFuture Tests

        [Fact]
        public void ValidateSignificantDateInFuture_WithDateInFuture_ReturnsSuccess()
        {
            // Arrange
            var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

            // Act
            var result = _validator.ValidateSignificantDateInFuture(futureDate);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDateInFuture_WithDateToday_ReturnsSuccess()
        {
            // Arrange
            var todayDate = DateOnly.FromDateTime(DateTime.Today);

            // Act
            var result = _validator.ValidateSignificantDateInFuture(todayDate);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDateInFuture_WithDateInPast_ReturnsError()
        {
            // Arrange
            var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

            // Act
            var result = _validator.ValidateSignificantDateInFuture(pastDate);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The Significant date must be in the future.", result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDateInFuture_WithNullDate_ReturnsSuccess()
        {
            // Arrange
            DateOnly? nullDate = null;

            // Act
            var result = _validator.ValidateSignificantDateInFuture(nullDate);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        #endregion

        #region ValidateSignificantDateNotSameAsCurrent Tests

        [Fact]
        public void ValidateSignificantDateNotSameAsCurrent_WithDifferentDate_ReturnsSuccess()
        {
            // Arrange
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var newDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var project = CreateTestProject(currentDate);

            // Act
            var result = _validator.ValidateSignificantDateNotSameAsCurrent(newDate, project);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDateNotSameAsCurrent_WithSameDate_ReturnsError()
        {
            // Arrange
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var project = CreateTestProject(currentDate);

            // Act
            var result = _validator.ValidateSignificantDateNotSameAsCurrent(currentDate, project);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The new date cannot be the same as the current date. Check you have entered the correct date.", result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDateNotSameAsCurrent_WithNullNewDate_ReturnsSuccess()
        {
            // Arrange
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var project = CreateTestProject(currentDate);
            DateOnly? nullDate = null;

            // Act
            var result = _validator.ValidateSignificantDateNotSameAsCurrent(nullDate, project);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDateNotSameAsCurrent_WithProjectHavingNullDate_ReturnsSuccess()
        {
            // Arrange
            var newDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var project = CreateTestProject(null);

            // Act
            var result = _validator.ValidateSignificantDateNotSameAsCurrent(newDate, project);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        #endregion

        #region ValidateSignificantDate Tests

        [Fact]
        public void ValidateSignificantDate_WithValidFutureDate_AndNoExistingProject_ReturnsSuccess()
        {
            // Arrange
            var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

            // Act
            var result = _validator.ValidateSignificantDate(futureDate);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDate_WithPastDate_ReturnsError()
        {
            // Arrange
            var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

            // Act
            var result = _validator.ValidateSignificantDate(pastDate);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The Significant date must be in the future.", result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDate_WithValidFutureDate_AndDifferentExistingDate_ReturnsSuccess()
        {
            // Arrange
            var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var existingProject = CreateTestProject(currentDate);

            // Act
            var result = _validator.ValidateSignificantDate(futureDate, existingProject);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDate_WithSameDateAsExisting_ReturnsError()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var existingProject = CreateTestProject(date);

            // Act
            var result = _validator.ValidateSignificantDate(date, existingProject);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The new date cannot be the same as the current date. Check you have entered the correct date.", result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDate_WithPastDate_ReturnsErrorRegardlessOfExistingProject()
        {
            // Arrange
            var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var existingProject = CreateTestProject(currentDate);

            // Act
            var result = _validator.ValidateSignificantDate(pastDate, existingProject);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The Significant date must be in the future.", result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDate_WithNullDate_ReturnsSuccess()
        {
            // Arrange
            DateOnly? nullDate = null;
            var existingProject = CreateTestProject(DateOnly.FromDateTime(DateTime.Today.AddDays(5)));

            // Act
            var result = _validator.ValidateSignificantDate(nullDate, existingProject);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Theory]
        [InlineData(1)] // Tomorrow
        [InlineData(30)] // Next month
        [InlineData(365)] // Next year
        public void ValidateSignificantDate_WithVariousFutureDates_ReturnsSuccess(int daysInFuture)
        {
            // Arrange
            var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(daysInFuture));
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var existingProject = CreateTestProject(currentDate);

            // Act
            var result = _validator.ValidateSignificantDate(futureDate, existingProject);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Theory]
        [InlineData(-1)] // Yesterday
        [InlineData(-30)] // Last month
        [InlineData(-365)] // Last year
        public void ValidateSignificantDate_WithVariousPastDates_ReturnsError(int daysInPast)
        {
            // Arrange
            var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(daysInPast));

            // Act
            var result = _validator.ValidateSignificantDate(pastDate);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The Significant date must be in the future.", result.ErrorMessage);
        }

        #endregion

        #region ValidationResult Tests

        [Fact]
        public void ValidationResult_Success_CreatesValidResult()
        {
            // Act
            var result = ValidationResult.Success();

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void ValidationResult_Error_CreatesInvalidResultWithMessage()
        {
            // Arrange
            const string errorMessage = "Test error message";

            // Act
            var result = ValidationResult.Error(errorMessage);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(errorMessage, result.ErrorMessage);
        }

        #endregion

        #region Helper Methods

        private static ProjectDto CreateTestProject(DateOnly? significantDate)
        {
            return new ProjectDto
            {
                Id = new ProjectId(Guid.NewGuid()),
                Urn = new Urn(123456),
                Type = ProjectType.Conversion,
                SignificantDate = significantDate,
                SignificantDateProvisional = false,
                State = ProjectState.Active,
                CreatedAt = DateTime.UtcNow,
                AssignedToId = new UserId(Guid.NewGuid())
            };
        }

        #endregion
    }
}