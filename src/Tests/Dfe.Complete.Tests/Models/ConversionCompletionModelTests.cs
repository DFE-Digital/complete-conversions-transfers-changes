using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectCompletion;

namespace Dfe.Complete.Tests.Models
{
    public class ConversionCompletionModelTests
    {
        private readonly ConversionCompletionModel _testClass;
        private DateOnly? NextMonthDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(months: 1));
        private DateOnly? PreviousMonthDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(months: -1));

        public ConversionCompletionModelTests()
        {
            _testClass = new();
        }

        [Theory]
        [InlineData(TaskListStatus.InProgress, TaskListStatus.InProgress, "12121212", true)]
        [InlineData(TaskListStatus.NotStarted, TaskListStatus.NotStarted, "12121212", true)]
        [InlineData(TaskListStatus.NotApplicable, TaskListStatus.NotApplicable, "12121212", true)]
        [InlineData(TaskListStatus.Completed, TaskListStatus.Completed, null, true)]
        [InlineData(TaskListStatus.Completed, TaskListStatus.Completed, "12121212", false)]
        public void Is_InValid_When_TasksConditionNotMet(TaskListStatus academyOpenedDateTaskStatus, TaskListStatus allConditionsMetTaskStatus, string incomingTrustUkprn, bool expected)
        {
            // Arrange            
            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.ConfirmDateAcademyOpened = academyOpenedDateTaskStatus;
            _testClass.ConfirmAllConditionsHaveBeenMet = allConditionsMetTaskStatus;
            _testClass.IncomingTrustUkprn = incomingTrustUkprn;

            // Act
            var result = _testClass.Validate();

            // Assert
            Assert.Equal(expected, result.Count > 0);
        }

        [Fact]
        public void Is_InValid_When_TasksConditionMet_ConversionDateIsFutureDate()
        {
            // Arrange                        
            var statusCompleted = TaskListStatus.Completed;
            _testClass.ConversionOrTransferDate = NextMonthDate;
            _testClass.ConfirmDateAcademyOpened = statusCompleted;
            _testClass.ConfirmAllConditionsHaveBeenMet = statusCompleted;

            // Act
            var result = _testClass.Validate();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Is_Valid_When_TasksConditionMet_ConversionDateIsPastDate()
        {
            // Arrange            
            var statusCompleted = TaskListStatus.Completed;

            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.ConfirmDateAcademyOpened = statusCompleted;
            _testClass.ConfirmAllConditionsHaveBeenMet = statusCompleted;
            _testClass.IncomingTrustUkprn = "12121212";

            // Act
            var result = _testClass.Validate();

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Test_When_TasksConditionMet_ConversionDateIsPastDate_WithProvisionalState(bool Provisional, bool expected)
        {
            // Arrange            
            var statusCompleted = TaskListStatus.Completed;

            _testClass.IsConversionOrTransferDateProvisional = Provisional;
            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.ConfirmDateAcademyOpened = statusCompleted;
            _testClass.ConfirmAllConditionsHaveBeenMet = statusCompleted;
            _testClass.IncomingTrustUkprn = "12121212";

            // Act
            var result = _testClass.Validate();

            // Assert            
            Assert.Equal(expected, result.Count > 0);
        }
    }
}
