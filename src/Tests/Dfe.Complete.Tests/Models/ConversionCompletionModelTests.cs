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
        [InlineData(TaskListStatus.InProgress, TaskListStatus.InProgress, false)]
        [InlineData(TaskListStatus.NotStarted, TaskListStatus.NotStarted, false)]
        [InlineData(TaskListStatus.NotApplicable, TaskListStatus.NotApplicable, false)]        
        [InlineData(TaskListStatus.Completed, TaskListStatus.Completed, true)]       
        public void Is_InValid_When_TasksConditionNotMet(TaskListStatus academyOpenedDateTaskStatus, TaskListStatus allConditionsMetTaskStatus, bool expected)
        {
            // Arrange            
            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.ConfirmDateAcademyOpened = academyOpenedDateTaskStatus;
            _testClass.ConfirmAllConditionsHaveBeenMet = allConditionsMetTaskStatus;

            // Assert
            Assert.Equal(expected, _testClass.IsValid);
        }

        [Fact]        
        public void Is_InValid_When_TasksConditionMet_ConversionDateIsFutureDate()
        {
            // Arrange                        
            var statusCompleted = TaskListStatus.Completed;
            _testClass.ConversionOrTransferDate = NextMonthDate;
            _testClass.ConfirmDateAcademyOpened = statusCompleted;
            _testClass.ConfirmAllConditionsHaveBeenMet = statusCompleted;

            // Assert
            Assert.False(_testClass.IsValid);
        }

        [Fact]
        public void Is_Valid_When_TasksConditionMet_ConversionDateIsPastDate()
        {
            // Arrange            
            var statusCompleted = TaskListStatus.Completed;
            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.ConfirmDateAcademyOpened = statusCompleted;
            _testClass.ConfirmAllConditionsHaveBeenMet = statusCompleted;

            // Assert
            Assert.True(_testClass.IsValid);
        }
    }
}
