using Dfe.Complete.Constants;

namespace Dfe.Complete.Models.ProjectCompletion;

public class ConversionCompletionModel : CompletionModel
{
    private bool AcademyOpenedDateTaskCompleted => ConfirmDateAcademyOpened == TaskListStatus.Completed;
    private bool AllConditionsMetTaskCompleted => ConfirmAllConditionsHaveBeenMet == TaskListStatus.Completed;

    public TaskListStatus ConfirmAllConditionsHaveBeenMet { get; set; }
    public TaskListStatus ConfirmDateAcademyOpened { get; set; }

    public List<string> Validate()
    {
        List<string> validationErrors = new List<string>();

        if (!DateConfirmedAndInThePast)
            validationErrors.Add(ValidationConstants.ConversionDateInPast);

        if (!AcademyOpenedDateTaskCompleted)
            validationErrors.Add(ValidationConstants.AcademyOpenedDateComplete);

        if (!AllConditionsMetTaskCompleted)
            validationErrors.Add(ValidationConstants.AllConditionsMetComplete);

        if (!IncomingTrustUkprnEntered)
            validationErrors.Add(ValidationConstants.IncomingTrustUkprnMissing);

        return validationErrors;
    }
}
