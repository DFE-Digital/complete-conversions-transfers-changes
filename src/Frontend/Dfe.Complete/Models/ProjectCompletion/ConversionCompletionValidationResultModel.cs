namespace Dfe.Complete.Models.ProjectCompletion;

public class ConversionCompletionValidationResultModel : CompletionValidationResultModel
{   
    public bool AcademyOpenedDateTaskCompleted { get; set; }
    public bool AllConditionsMetTaskCompleted { get; set; }
    public bool ShowNotification =>  DateConfirmedAndInThePast || AcademyOpenedDateTaskCompleted || AllConditionsMetTaskCompleted;
}
