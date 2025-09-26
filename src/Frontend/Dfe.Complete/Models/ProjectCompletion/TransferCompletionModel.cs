using Dfe.Complete.Constants;

namespace Dfe.Complete.Models.ProjectCompletion;

public class TransferCompletionModel : CompletionModel
{   
    private bool AuthorityToProceedTaskCompleted => ConfirmThisTransferHasAuthorityToProceed == TaskListStatus.Completed;
    private bool ExpenditureCertificateTaskCompleted => DeclarationOfExpenditureCertificate == TaskListStatus.Completed;
    private bool AcademyTransferDateTaskCompleted => ConfirmDateAcademyTransferred == TaskListStatus.Completed;

    public TaskListStatus ConfirmThisTransferHasAuthorityToProceed { get; set; }
    public TaskListStatus ConfirmDateAcademyTransferred { get; set; }
    public TaskListStatus DeclarationOfExpenditureCertificate { get; set; }

    public List<string> Validate()
    {
        List<string> validationErrors = new List<string>();

        if (!DateConfirmedAndInThePast)
            validationErrors.Add(ValidationConstants.TransferDateInPast);

        if (!AuthorityToProceedTaskCompleted)
            validationErrors.Add(ValidationConstants.AuthorityToProceedComplete);

        if (!ExpenditureCertificateTaskCompleted)
            validationErrors.Add(ValidationConstants.ExpenditureCertificateComplete);

        if (!AcademyTransferDateTaskCompleted)
            validationErrors.Add(ValidationConstants.AcademyTransferDateComplete);

        return validationErrors;
    }
   
}
