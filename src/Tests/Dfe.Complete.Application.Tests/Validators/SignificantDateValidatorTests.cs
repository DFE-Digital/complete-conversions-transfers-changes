using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Validation;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Xunit;

namespace Dfe.Complete.Application.Tests.Validators
{
    /// <summary>
    /// Tests for SignificantDateValidator
    /// Validates business rules for project significant dates and payroll deadline dates
    /// </summary>
    public class SignificantDateValidatorTests
    {
        private readonly SignificantDateValidator _validator;

        public SignificantDateValidatorTests()
        {
            _validator = new SignificantDateValidator();
        }

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

        #region ValidateSignificantDate with PayrollDeadline Tests

        [Fact]
        public void ValidateSignificantDate_WithPayrollDeadline_ValidFutureDate_AndAfterPayroll_ReturnsSuccess()
        {
            // Arrange
            var payrollDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var significantDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(15));
            var existingProject = CreateTestProject(currentDate);

            // Act
            var result = _validator.ValidateSignificantDate(significantDate, existingProject, payrollDate);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateSignificantDate_WithPayrollDeadline_SignificantDateInPast_ReturnsError()
        {
            // Arrange
            var payrollDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var significantDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            var existingProject = CreateTestProject(DateOnly.FromDateTime(DateTime.Today.AddDays(15)));

            // Act
            var result = _validator.ValidateSignificantDate(significantDate, existingProject, payrollDate);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The Significant date must be in the future.", result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDate_WithPayrollDeadline_SignificantDateSameAsCurrent_ReturnsError()
        {
            // Arrange
            var payrollDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var significantDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var existingProject = CreateTestProject(significantDate);

            // Act
            var result = _validator.ValidateSignificantDate(significantDate, existingProject, payrollDate);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The new date cannot be the same as the current date. Check you have entered the correct date.", result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDate_WithPayrollDeadline_SignificantDateBeforePayroll_ReturnsError()
        {
            // Arrange
            var payrollDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var significantDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(15));
            var existingProject = CreateTestProject(currentDate);

            // Act
            var result = _validator.ValidateSignificantDate(significantDate, existingProject, payrollDate);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The significant date must be after the payroll deadline.", result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDate_WithPayrollDeadline_SignificantDateSameAsPayroll_ReturnsError()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(15));
            var existingProject = CreateTestProject(currentDate);

            // Act
            var result = _validator.ValidateSignificantDate(date, existingProject, date);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The significant date must be after the payroll deadline.", result.ErrorMessage);
        }

        [Fact]
        public void ValidateSignificantDate_WithPayrollDeadline_NullPayrollDate_OnlyValidatesBasicRules()
        {
            // Arrange
            var significantDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var currentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(15));
            var existingProject = CreateTestProject(currentDate);
            DateOnly? nullPayrollDate = null;

            // Act
            var result = _validator.ValidateSignificantDate(significantDate, existingProject, nullPayrollDate);

            // Assert
            Assert.True(result.IsValid);
        }

        #endregion

        #region ValidatePayrollDeadline Tests

        [Fact]
        public void ValidatePayrollDeadline_WithValidPayrollDateBeforeSignificantDate_ReturnsSuccess()
        {
            // Arrange
            var payrollDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var significantDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var project = CreateTestProject(significantDate);

            // Act
            var result = _validator.ValidatePayrollDeadline(payrollDate, project);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidatePayrollDeadline_WithPayrollDateInPast_ReturnsError()
        {
            // Arrange
            var payrollDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            var significantDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var project = CreateTestProject(significantDate);

            // Act
            var result = _validator.ValidatePayrollDeadline(payrollDate, project);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The payroll deadline must be in the future.", result.ErrorMessage);
        }

        [Fact]
        public void ValidatePayrollDeadline_WithPayrollDateAfterSignificantDate_ReturnsError()
        {
            // Arrange
            var payrollDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var significantDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var project = CreateTestProject(significantDate);

            // Act
            var result = _validator.ValidatePayrollDeadline(payrollDate, project);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("The payroll deadline must be before the significant date.", result.ErrorMessage);
        }

        [Fact]
        public void ValidatePayrollDeadline_WithNullPayrollDate_ReturnsSuccess()
        {
            // Arrange
            DateOnly? nullPayrollDate = null;
            var significantDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            var project = CreateTestProject(significantDate);

            // Act
            var result = _validator.ValidatePayrollDeadline(nullPayrollDate, project);

            // Assert
            Assert.True(result.IsValid);
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