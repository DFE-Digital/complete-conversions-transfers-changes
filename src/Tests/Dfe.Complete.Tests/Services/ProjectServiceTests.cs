using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Services.Project;

namespace Dfe.Complete.Tests.Services
{
    public class ProjectServiceTests
    {
        private readonly ProjectService projectServiceMock = new();
        private DateOnly NextMonthDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(months: 1));
        private DateOnly LastMonthDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(months: -1));

        [Theory]        
        [InlineData(TaskListStatus.NotApplicable)]
        [InlineData(TaskListStatus.NotStarted)]
        [InlineData(TaskListStatus.InProgress)]
        public void GetTransferProjectCompletionResult_InValidData_ShouldReturn_Invalid__With_ValidationMessages(TaskListStatus taskListStatus)
        {            
            // Arrange           
            var taskList = new TransferTaskListViewModel
            {
                ConfirmThisTransferHasAuthorityToProceed = taskListStatus,
                ConfirmDateAcademyTransferred = taskListStatus,
                DeclarationOfExpenditureCertificate = taskListStatus
            };            
             
            // Act
            var result = projectServiceMock.GetTransferProjectCompletionValidationResult(NextMonthDate, taskList);

            // Assert            
            Assert.Contains(ValidationConstants.TransferDateInPast, result);
            Assert.Contains(ValidationConstants.AcademyTransferDateComplete, result);
            Assert.Contains(ValidationConstants.AuthorityToProceedComplete, result);
            Assert.Contains(ValidationConstants.ExpenditureCertificateComplete, result);
        }

        [Theory]
        [InlineData(TaskListStatus.Completed)]        
        public void GetTransferProjectCompletionResult_ValidData_ShouldReturn_Valid_NoValidationMessages(TaskListStatus taskListStatus)
        {
            // Arrange            
            var taskList = new TransferTaskListViewModel
            {
                ConfirmThisTransferHasAuthorityToProceed = taskListStatus,
                ConfirmDateAcademyTransferred = taskListStatus,
                DeclarationOfExpenditureCertificate = taskListStatus
            };

            // Act
            var result = projectServiceMock.GetTransferProjectCompletionValidationResult(LastMonthDate, taskList);

            // Assert                    
            Assert.DoesNotContain(ValidationConstants.TransferDateInPast, result);
            Assert.DoesNotContain(ValidationConstants.AcademyTransferDateComplete, result);
            Assert.DoesNotContain(ValidationConstants.AuthorityToProceedComplete, result);
            Assert.DoesNotContain(ValidationConstants.ExpenditureCertificateComplete, result);
        }

        [Theory]
        [InlineData(TaskListStatus.NotApplicable)]
        [InlineData(TaskListStatus.NotStarted)]
        [InlineData(TaskListStatus.InProgress)]
        public void GetConversionProjectCompletionResult_InValidData_ShouldReturn_Invalid_WithValidationMessages(TaskListStatus taskListStatus)
        {
            // Arrange           
            var taskList = new ConversionTaskListViewModel
            {
                ConfirmAllConditionsHaveBeenMet = taskListStatus,
                ConfirmDateAcademyOpened = taskListStatus,                
            };

            // Act
            var result = projectServiceMock.GetConversionProjectCompletionValidationResult(NextMonthDate, taskList);

            // Assert           
            Assert.Contains(ValidationConstants.ConversionDateInPast, result);
            Assert.Contains(ValidationConstants.AllConditionsMetComplete, result);
            Assert.Contains(ValidationConstants.AcademyOpenedDateComplete, result);            
        }

        [Theory]
        [InlineData(TaskListStatus.Completed)]
        public void GetConversionProjectCompletionResult_ValidData_ShouldReturn_ValidState_NoValidationMessages(TaskListStatus taskListStatus)
        {
            // Arrange            
            var taskList = new ConversionTaskListViewModel
            {
                ConfirmAllConditionsHaveBeenMet = taskListStatus,
                ConfirmDateAcademyOpened = taskListStatus,
            };

            // Act
            var result = projectServiceMock.GetConversionProjectCompletionValidationResult(LastMonthDate, taskList);

            // Assert            
            Assert.DoesNotContain(ValidationConstants.ConversionDateInPast, result);
            Assert.DoesNotContain(ValidationConstants.AllConditionsMetComplete, result);
            Assert.DoesNotContain(ValidationConstants.AcademyOpenedDateComplete, result);            
        }
    }
}
