using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectCompletion;

namespace Dfe.Complete.Services.Project;

public class ProjectService : IProjectService
{
    public List<string> GetTransferProjectCompletionValidationResult(DateOnly? SignificantDate, TransferTaskListViewModel taskList)
    {
        TransferCompletionModel transferCompletionModel = new();

        transferCompletionModel.ConversionOrTransferDate = SignificantDate;        
        transferCompletionModel.ConfirmThisTransferHasAuthorityToProceed = taskList.ConfirmThisTransferHasAuthorityToProceed;
        transferCompletionModel.ConfirmDateAcademyTransferred = taskList.ConfirmDateAcademyTransferred;
        transferCompletionModel.DeclarationOfExpenditureCertificate = taskList.DeclarationOfExpenditureCertificate;          
       
        return transferCompletionModel.Validate();
    }

    public List<string> GetConversionProjectCompletionValidationResult(DateOnly? SignificantDate, ConversionTaskListViewModel taskList)
    {
        ConversionCompletionModel conversionCompletionModel = new();

        conversionCompletionModel.ConversionOrTransferDate = SignificantDate;        
        conversionCompletionModel.ConfirmAllConditionsHaveBeenMet = taskList.ConfirmAllConditionsHaveBeenMet;
        conversionCompletionModel.ConfirmDateAcademyOpened = taskList.ConfirmDateAcademyOpened;

        return conversionCompletionModel.Validate();
    }

}

