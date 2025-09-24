using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectCompletion;

namespace Dfe.Complete.Tests.Models
{
    public class TransferCompletionModelTests
    {
        private DateOnly NextMonthDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(months: 1));
        private DateOnly PreviousMonthDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(months: -1));
        private readonly TransferCompletionModel _testClass;

        public TransferCompletionModelTests()
        {   
            _testClass = new();
        }       

        [Theory]
        [InlineData(TaskListStatus.InProgress, TaskListStatus.InProgress, TaskListStatus.InProgress, false)]
        [InlineData(TaskListStatus.NotStarted, TaskListStatus.NotStarted, TaskListStatus.NotStarted, false)]
        [InlineData(TaskListStatus.NotApplicable, TaskListStatus.NotApplicable, TaskListStatus.NotApplicable, false)]
        [InlineData(TaskListStatus.Completed, TaskListStatus.Completed, TaskListStatus.Completed, true)]
        public void Is_InValid_When_TasksConditionNotMet(TaskListStatus confirmThisTransferHasAuthorityToProceed, TaskListStatus confirmDateAcademyTransferred, TaskListStatus declarationOfExpenditureCertificate, bool expected)
        {
            // Arrange            
            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.ConfirmThisTransferHasAuthorityToProceed = confirmThisTransferHasAuthorityToProceed;
            _testClass.ConfirmDateAcademyTransferred = confirmDateAcademyTransferred;
            _testClass.DeclarationOfExpenditureCertificate = declarationOfExpenditureCertificate;

            // Assert
            Assert.Equal(expected, _testClass.IsValid);
        }

        [Fact]
        public void Is_InValid_When_TasksConditionMet_TransferDateIsFutureDate()
        {
            // Arrange            
            var statusCompleted = TaskListStatus.Completed;

            _testClass.ConversionOrTransferDate = NextMonthDate;
            _testClass.ConfirmThisTransferHasAuthorityToProceed = statusCompleted;
            _testClass.ConfirmDateAcademyTransferred = statusCompleted;
            _testClass.DeclarationOfExpenditureCertificate = statusCompleted;           

            // Assert
            Assert.False(_testClass.IsValid);
        }

        [Fact]
        public void Is_Valid_When_TasksConditionMet_TransferDateIsPastDate()
        {
            // Arrange            
            var statusCompleted = TaskListStatus.Completed;

            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.ConfirmThisTransferHasAuthorityToProceed = statusCompleted;
            _testClass.ConfirmDateAcademyTransferred = statusCompleted;
            _testClass.DeclarationOfExpenditureCertificate = statusCompleted;           

            // Assert
            Assert.True(_testClass.IsValid);
        }
    }
}
