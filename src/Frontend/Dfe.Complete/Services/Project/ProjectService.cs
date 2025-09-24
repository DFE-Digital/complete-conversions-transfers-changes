using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectCompletion;

namespace Dfe.Complete.Services.Project;

public class ProjectService : IProjectService
{
    public TransferCompletionModel GetTransferProjectCompletionResult(DateOnly? SignificantDate, TransferTaskListViewModel taskList)
    {
        TransferCompletionModel transferCompletionModel = new();
        transferCompletionModel.ConversionOrTransferDate = SignificantDate;        
        transferCompletionModel.ConfirmThisTransferHasAuthorityToProceed = taskList.ConfirmThisTransferHasAuthorityToProceed;
        transferCompletionModel.ConfirmDateAcademyTransferred = taskList.ConfirmDateAcademyTransferred;
        transferCompletionModel.DeclarationOfExpenditureCertificate = taskList.DeclarationOfExpenditureCertificate;          

        if(!transferCompletionModel.DateConfirmedAndInThePast)
            transferCompletionModel.ValidationErrors.Add(ValidationConstants.TransferDateInPast);

        if (!transferCompletionModel.AuthorityToProceedCompleteTaskCompleted)
            transferCompletionModel.ValidationErrors.Add(ValidationConstants.AuthorityToProceedComplete);

        if (!transferCompletionModel.ExpendentureCertificateTaskCompleted)
            transferCompletionModel.ValidationErrors.Add(ValidationConstants.ExpenditureCertificateComplete);

        if (!transferCompletionModel.AcademyTransferDateTaskCompleted)
            transferCompletionModel.ValidationErrors.Add(ValidationConstants.AcademyTransferDateComplete);

        return transferCompletionModel;
    }

    public ConversionCompletionModel GetConversionProjectCompletionResult(DateOnly? SignificantDate, ConversionTaskListViewModel taskList)
    {
        ConversionCompletionModel conversionCompletionModel = new();
        conversionCompletionModel.ConversionOrTransferDate = SignificantDate;
        
        conversionCompletionModel.ConfirmAllConditionsHaveBeenMet = taskList.ConfirmAllConditionsHaveBeenMet;
        conversionCompletionModel.ConfirmDateAcademyOpened = taskList.ConfirmDateAcademyOpened;

        if (!conversionCompletionModel.DateConfirmedAndInThePast)
            conversionCompletionModel.ValidationErrors.Add(ValidationConstants.ConversionDateInPast);

        if (!conversionCompletionModel.AllConditionsMetTaskCompleted)
            conversionCompletionModel.ValidationErrors.Add(ValidationConstants.AllConditionsMetComplete);

        if (!conversionCompletionModel.AcademyOpenedDateTaskCompleted)
            conversionCompletionModel.ValidationErrors.Add(ValidationConstants.AcademyOpenedDateComplete);

        return conversionCompletionModel;
    }

}

