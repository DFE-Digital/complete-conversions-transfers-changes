namespace Dfe.Complete.Models.ProjectCompletion;

public class TransferCompletionValidationResultModel : CompletionValidationResultModel
{   
    public bool AuthorityToProceedCompleteTaskCompleted { get; set; }
    public bool ExpendentureCertificateTaskCompleted { get; set; }
    public bool AcademyTransferDateTaskCompleted { get; set; }

    public bool ShowNotification => 
    DateConfirmedAndInThePast ||
    AuthorityToProceedCompleteTaskCompleted ||
    ExpendentureCertificateTaskCompleted ||
    AcademyTransferDateTaskCompleted;
}
