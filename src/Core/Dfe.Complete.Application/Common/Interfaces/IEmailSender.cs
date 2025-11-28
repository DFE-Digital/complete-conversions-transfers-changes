using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for sending emails through a notification service.
    /// Technology-agnostic abstraction that can be implemented by any email provider.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="message">The email message to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing EmailSendResult on success or error details on failure</returns>
        Task<Result<EmailSendResult>> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
    }
}

