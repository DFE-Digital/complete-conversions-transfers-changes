using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectCompletion;

namespace Dfe.Complete.Services.Project;

public class ProjectService : IProjectService
{
    public List<string> GetTransferProjectCompletionValidationResult(DateOnly? SignificantDate, bool SignificantDateProvisional, TransferTaskListViewModel taskList, string? IncomingTrustUkprn, bool IsOnHold)
    {
        TransferCompletionModel transferCompletionModel = new()
        {
            ConversionOrTransferDate = SignificantDate,
            IsConversionOrTransferDateProvisional = SignificantDateProvisional,
            ConfirmThisTransferHasAuthorityToProceed = taskList.ConfirmThisTransferHasAuthorityToProceed,
            ConfirmDateAcademyTransferred = taskList.ConfirmDateAcademyTransferred,
            DeclarationOfExpenditureCertificate = taskList.DeclarationOfExpenditureCertificate,
            IncomingTrustUkprn = IncomingTrustUkprn,
            IsOnHold = IsOnHold
        };

        return transferCompletionModel.Validate();
    }

    public List<string> GetConversionProjectCompletionValidationResult(DateOnly? SignificantDate, bool SignificantDateProvisional, ConversionTaskListViewModel taskList, string? IncomingTrustUkprn, bool IsOnHold)
    {
        ConversionCompletionModel conversionCompletionModel = new()
        {
            ConversionOrTransferDate = SignificantDate,
            IsConversionOrTransferDateProvisional = SignificantDateProvisional,
            ConfirmAllConditionsHaveBeenMet = taskList.ConfirmAllConditionsHaveBeenMet,
            ConfirmDateAcademyOpened = taskList.ConfirmDateAcademyOpened,
            IncomingTrustUkprn = IncomingTrustUkprn,
            IsOnHold = IsOnHold
        };

        return conversionCompletionModel.Validate();
    }

}

