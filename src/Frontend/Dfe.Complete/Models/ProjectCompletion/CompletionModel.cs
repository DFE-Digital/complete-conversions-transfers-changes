namespace Dfe.Complete.Models.ProjectCompletion;

public class CompletionModel
{
    public DateOnly? ConversionOrTransferDate { get; set; }
    protected bool DateConfirmedAndInThePast  => ConversionOrTransferDate.HasValue && ConversionOrTransferDate <= DateOnly.FromDateTime(DateTime.Today);
}
