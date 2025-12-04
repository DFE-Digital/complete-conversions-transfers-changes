namespace Dfe.Complete.Models.ProjectCompletion;

public class CompletionModel
{
    public DateOnly? ConversionOrTransferDate { get; set; }
    public bool IsConversionOrTransferDateProvisional { get; set; }
    public string? IncomingTrustUkprn { get; set; } = string.Empty;
    protected bool DateConfirmedAndInThePast => !IsConversionOrTransferDateProvisional && (ConversionOrTransferDate.HasValue && ConversionOrTransferDate <= DateOnly.FromDateTime(DateTime.Today));
    protected bool IncomingTrustUkprnEntered => !string.IsNullOrWhiteSpace(IncomingTrustUkprn);

}
