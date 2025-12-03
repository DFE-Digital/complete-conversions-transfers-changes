using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectCompletion;

namespace Dfe.Complete.Services.Project;

public class ProjectService : IProjectService
{
    public List<string> GetTransferProjectCompletionValidationResult(DateOnly? SignificantDate, bool SignificantDateProvisional, TransferTaskListViewModel taskList, string? IncomingTrustUkprn)
    {
        TransferCompletionModel transferCompletionModel = new()
        {
            ConversionOrTransferDate = SignificantDate,
            IsConversionOrTransferDateProvisional = SignificantDateProvisional,
            ConfirmThisTransferHasAuthorityToProceed = taskList.ConfirmThisTransferHasAuthorityToProceed,
            ConfirmDateAcademyTransferred = taskList.ConfirmDateAcademyTransferred,
            DeclarationOfExpenditureCertificate = taskList.DeclarationOfExpenditureCertificate,
            IncomingTrustUkprn = IncomingTrustUkprn
        };

        return transferCompletionModel.Validate();
    }

    public List<string> GetConversionProjectCompletionValidationResult(DateOnly? SignificantDate, bool SignificantDateProvisional, ConversionTaskListViewModel taskList, string? IncomingTrustUkprn)
    {
        ConversionCompletionModel conversionCompletionModel = new()
        {
            ConversionOrTransferDate = SignificantDate,
            IsConversionOrTransferDateProvisional = SignificantDateProvisional,
            ConfirmAllConditionsHaveBeenMet = taskList.ConfirmAllConditionsHaveBeenMet,
            ConfirmDateAcademyOpened = taskList.ConfirmDateAcademyOpened,
            IncomingTrustUkprn = IncomingTrustUkprn
        };

        return conversionCompletionModel.Validate();
    }

}

