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
        [InlineData(TaskListStatus.InProgress, TaskListStatus.InProgress, true)]
        [InlineData(TaskListStatus.NotStarted, TaskListStatus.NotStarted, true)]
        [InlineData(TaskListStatus.NotApplicable, TaskListStatus.NotApplicable, true)]        
        [InlineData(TaskListStatus.Completed, TaskListStatus.Completed, false)]       
        public void Is_InValid_When_TasksConditionNotMet(TaskListStatus academyOpenedDateTaskStatus, TaskListStatus allConditionsMetTaskStatus, bool expected)
        {
            // Arrange            
            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.ConfirmDateAcademyOpened = academyOpenedDateTaskStatus;
            _testClass.ConfirmAllConditionsHaveBeenMet = allConditionsMetTaskStatus;

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

            // Act
            var result = _testClass.Validate();

            // Assert            
            Assert.Equal(expected, result.Count > 0);
        }
    }
}
