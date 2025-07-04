namespace Dfe.Complete.Application.Projects.Models.TransferTasks;

public record TransferFormMTaskDataDto(
    bool? FormMNotApplicable,
    bool? FormMReceivedFormM,
    bool? FormMReceivedTitlePlans,
    bool? FormMCleared,
    bool? FormMSigned,
    bool? FormMSaved
);
