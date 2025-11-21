namespace Dfe.Complete.Models.ProjectCompletion;

public class CompletionModel
{
    public DateOnly? ConversionOrTransferDate { get; set; }
    public bool IsConversionOrTransferDateProvisional { get; set; }
    protected bool DateConfirmedAndInThePast => !IsConversionOrTransferDateProvisional && (ConversionOrTransferDate.HasValue && ConversionOrTransferDate <= DateOnly.FromDateTime(DateTime.Today));
}
