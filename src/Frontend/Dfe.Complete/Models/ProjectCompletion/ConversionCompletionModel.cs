using static Dfe.Complete.Models.Links;

namespace Dfe.Complete.Models.ProjectCompletion;

public class ConversionCompletionModel : CompletionModel
{   
    public bool AcademyOpenedDateTaskCompleted => ConfirmDateAcademyOpened == TaskListStatus.Completed;
    public bool AllConditionsMetTaskCompleted => ConfirmAllConditionsHaveBeenMet == TaskListStatus.Completed;

    public TaskListStatus ConfirmAllConditionsHaveBeenMet { get; set; }
    public TaskListStatus ConfirmDateAcademyOpened { get; set; }

    public bool IsValid =>  DateConfirmedAndInThePast &&  AcademyOpenedDateTaskCompleted &&  AllConditionsMetTaskCompleted;
}
