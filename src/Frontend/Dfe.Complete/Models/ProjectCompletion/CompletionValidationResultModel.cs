namespace Dfe.Complete.Models.ProjectCompletion;

public class CompletionValidationResultModel
{
    public bool DateConfirmedAndInThePast { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}
