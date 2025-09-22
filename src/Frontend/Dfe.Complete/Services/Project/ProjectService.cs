using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectCompletion;

namespace Dfe.Complete.Services.Project;

public class ProjectService : IProjectService
{
    public TransferCompletionValidationResultModel GetTransferProjectCompletionResult(DateOnly? SignificantDate, TransferTaskListViewModel taskList)
    {
        TransferCompletionValidationResultModel validationResult = new();

        // check date is in the past
        if (SignificantDate.HasValue && SignificantDate < DateOnly.FromDateTime(DateTime.Now))
        {
            validationResult.DateConfirmedAndInThePast = true;
        }

        // check all below tasks completed
        validationResult.AuthorityToProceedCompleteTaskCompleted = taskList.ConfirmThisTransferHasAuthorityToProceed == TaskListStatus.Completed;
        validationResult.AcademyTransferDateTaskCompleted = taskList.ConfirmDateAcademyTransferred == TaskListStatus.Completed;
        validationResult.ExpendentureCertificateTaskCompleted = taskList.DeclarationOfExpenditureCertificate == TaskListStatus.Completed;          

        if(!validationResult.DateConfirmedAndInThePast)         
            validationResult.ValidationErrors.Add(ValidationConstants.TransferDateInPast);

        if (!validationResult.AuthorityToProceedCompleteTaskCompleted)
            validationResult.ValidationErrors.Add(ValidationConstants.AuthorityToProceedComplete);

        if (!validationResult.ExpendentureCertificateTaskCompleted)
            validationResult.ValidationErrors.Add(ValidationConstants.ExpenditureCertificateComplete);

        if (!validationResult.AcademyTransferDateTaskCompleted)
            validationResult.ValidationErrors.Add(ValidationConstants.AcademyTransferDateComplete);

        return validationResult;
    }

    public ConversionCompletionValidationResultModel GetConversionProjectCompletionResult(DateOnly? SignificantDate, ConversionTaskListViewModel taskList)
    {
        ConversionCompletionValidationResultModel validationResult = new();

        // check date is in the past
        if (SignificantDate.HasValue && SignificantDate < DateOnly.FromDateTime(DateTime.Now))
        {
            validationResult.DateConfirmedAndInThePast = true;
        }

        // check all below tasks completed
        validationResult.AllConditionsMetTaskCompleted = taskList.ConfirmAllConditionsHaveBeenMet == TaskListStatus.Completed;
        validationResult.AcademyOpenedDateTaskCompleted = taskList.ConfirmDateAcademyOpened == TaskListStatus.Completed;

        if (!validationResult.DateConfirmedAndInThePast)
            validationResult.ValidationErrors.Add(ValidationConstants.ConversionDateInPast);

        if (!validationResult.AllConditionsMetTaskCompleted)
            validationResult.ValidationErrors.Add(ValidationConstants.AllConditionsMetComplete);

        if (!validationResult.AcademyOpenedDateTaskCompleted)
            validationResult.ValidationErrors.Add(ValidationConstants.AcademyOpenedDateComplete);

        return validationResult;
    }

}

