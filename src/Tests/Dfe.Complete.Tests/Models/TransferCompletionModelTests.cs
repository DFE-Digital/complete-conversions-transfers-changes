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
        [InlineData(TaskListStatus.InProgress, TaskListStatus.InProgress, TaskListStatus.InProgress, "12121212", true)]
        [InlineData(TaskListStatus.NotStarted, TaskListStatus.NotStarted, TaskListStatus.NotStarted, "12121212", true)]
        [InlineData(TaskListStatus.NotApplicable, TaskListStatus.NotApplicable, TaskListStatus.NotApplicable, "12121212", true)]
        [InlineData(TaskListStatus.Completed, TaskListStatus.Completed, TaskListStatus.NotApplicable, null, true)]
        [InlineData(TaskListStatus.Completed, TaskListStatus.Completed, TaskListStatus.NotApplicable, "12121212", false)]
        [InlineData(TaskListStatus.Completed, TaskListStatus.Completed, TaskListStatus.Completed, "12121212", false)]
        public void Is_InValid_When_TasksConditionNotMet(TaskListStatus confirmThisTransferHasAuthorityToProceed, TaskListStatus confirmDateAcademyTransferred, TaskListStatus declarationOfExpenditureCertificate, string incomingTrustUkprn, bool expected)
        {
            // Arrange            
            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.ConfirmThisTransferHasAuthorityToProceed = confirmThisTransferHasAuthorityToProceed;
            _testClass.ConfirmDateAcademyTransferred = confirmDateAcademyTransferred;
            _testClass.DeclarationOfExpenditureCertificate = declarationOfExpenditureCertificate;
            _testClass.IncomingTrustUkprn = incomingTrustUkprn;

            // Act
            var result = _testClass.Validate();

            // Assert
            Assert.Equal(expected, result.Count > 0);
        }

        [Fact]
        public void Test_InValid_When_TasksConditionMet_TransferDateIsFutureDate()
        {
            // Arrange            
            var statusCompleted = TaskListStatus.Completed;

            _testClass.ConversionOrTransferDate = NextMonthDate;
            _testClass.ConfirmThisTransferHasAuthorityToProceed = statusCompleted;
            _testClass.ConfirmDateAcademyTransferred = statusCompleted;
            _testClass.DeclarationOfExpenditureCertificate = statusCompleted;

            // Act
            var result = _testClass.Validate();

            // Assert
            Assert.NotEmpty(result);
        }


        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Test_When_TasksConditionMet_TransferDateIsPastDate_WithProvisionalState(bool Provisional, bool expected)
        {
            // Arrange            
            var statusCompleted = TaskListStatus.Completed;

            _testClass.ConversionOrTransferDate = PreviousMonthDate;
            _testClass.IsConversionOrTransferDateProvisional = Provisional;
            _testClass.ConfirmThisTransferHasAuthorityToProceed = statusCompleted;
            _testClass.ConfirmDateAcademyTransferred = statusCompleted;
            _testClass.DeclarationOfExpenditureCertificate = statusCompleted;
            _testClass.IncomingTrustUkprn = "12121212";

            // Act
            var result = _testClass.Validate();

            // Assert            
            Assert.Equal(expected, result.Count > 0);
        }
    }
}
