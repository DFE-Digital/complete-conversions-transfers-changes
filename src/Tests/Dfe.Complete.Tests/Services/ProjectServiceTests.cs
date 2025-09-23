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
            var result = projectServiceMock.GetTransferProjectCompletionResult(NextMonthDate, taskList);

            // Assert
            var validationErrors = result.ValidationErrors;
            Assert.False(result.IsValid);
            Assert.Contains(ValidationConstants.TransferDateInPast, validationErrors);
            Assert.Contains(ValidationConstants.AcademyTransferDateComplete, validationErrors);
            Assert.Contains(ValidationConstants.AuthorityToProceedComplete, validationErrors);
            Assert.Contains(ValidationConstants.ExpenditureCertificateComplete, validationErrors);
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
            var result = projectServiceMock.GetTransferProjectCompletionResult(LastMonthDate, taskList);

            // Assert
            var validationErrors = result.ValidationErrors;
            Assert.True(result.IsValid);            
            Assert.DoesNotContain(ValidationConstants.TransferDateInPast, validationErrors);
            Assert.DoesNotContain(ValidationConstants.AcademyTransferDateComplete, validationErrors);
            Assert.DoesNotContain(ValidationConstants.AuthorityToProceedComplete, validationErrors);
            Assert.DoesNotContain(ValidationConstants.ExpenditureCertificateComplete, validationErrors);
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
            var result = projectServiceMock.GetConversionProjectCompletionResult(NextMonthDate, taskList);

            // Assert
            var validationErrors = result.ValidationErrors;
            Assert.False(result.IsValid);
            Assert.Contains(ValidationConstants.ConversionDateInPast, validationErrors);
            Assert.Contains(ValidationConstants.AllConditionsMetComplete, validationErrors);
            Assert.Contains(ValidationConstants.AcademyOpenedDateComplete, validationErrors);            
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
            var result = projectServiceMock.GetConversionProjectCompletionResult(LastMonthDate, taskList);

            // Assert
            var validationErrors = result.ValidationErrors;
            Assert.True(result.IsValid);
            Assert.DoesNotContain(ValidationConstants.ConversionDateInPast, validationErrors);
            Assert.DoesNotContain(ValidationConstants.AllConditionsMetComplete, validationErrors);
            Assert.DoesNotContain(ValidationConstants.AcademyOpenedDateComplete, validationErrors);            
        }
    }
}
