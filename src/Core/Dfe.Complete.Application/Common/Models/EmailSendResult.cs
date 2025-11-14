namespace Dfe.Complete.Application.Common.Models
{
    /// <summary>
    /// Represents the result of a successful email send operation.
    /// Contains minimal information needed to track the email.
    /// Errors are handled via Result&lt;EmailSendResult&gt;.ErrorType.
    /// </summary>
    public record EmailSendResult(
        string ProviderMessageId,
        string Reference,
        DateTime SentAt);
}

