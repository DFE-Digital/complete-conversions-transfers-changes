namespace Dfe.Complete.Models.ProjectCompletion;

public class TransferCompletionModel : CompletionModel
{   
    public bool AuthorityToProceedTaskCompleted => ConfirmThisTransferHasAuthorityToProceed == TaskListStatus.Completed;
    public bool ExpendentureCertificateTaskCompleted => DeclarationOfExpenditureCertificate == TaskListStatus.Completed;
    public bool AcademyTransferDateTaskCompleted => ConfirmDateAcademyTransferred == TaskListStatus.Completed;

    public TaskListStatus ConfirmThisTransferHasAuthorityToProceed { get; set; }
    public TaskListStatus ConfirmDateAcademyTransferred { get; set; }
    public TaskListStatus DeclarationOfExpenditureCertificate { get; set; }

    public bool IsValid => 
    DateConfirmedAndInThePast &&
    AuthorityToProceedTaskCompleted &&
    ExpendentureCertificateTaskCompleted &&
    AcademyTransferDateTaskCompleted;
}
