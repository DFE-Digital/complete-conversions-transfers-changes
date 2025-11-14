using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Common.EventHandlers
{
    /// <summary>
    /// Handles UserCreatedEvent and sends welcome email to new user.
    /// Implements Ruby mailer: UserAccountMailer#new_account_added
    /// </summary>
    public class SendNewAccountEmailHandler(
        IEmailSender emailSender,
        ILogger<SendNewAccountEmailHandler> logger) : BaseEventHandler<UserCreatedEvent>(logger)
    {
        protected override async Task HandleEvent(
            UserCreatedEvent notification,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Sending new account email to user {UserId} ({Email})",
                notification.UserId,
                notification.Email);

            var emailAddress = EmailAddress.Create(notification.Email);

            var emailMessage = new EmailMessage(
                To: emailAddress,
                TemplateKey: "NewAccountAdded",
                Personalisation: new Dictionary<string, string>
                {
                    { "first_name", notification.FirstName }
                });

            var result = await emailSender.SendAsync(emailMessage, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "New account email sent successfully to {Email}. MessageId: {MessageId}",
                    notification.Email,
                    result.Value?.ProviderMessageId);
            }
            else
            {
                logger.LogError(
                    "Failed to send new account email to {Email}. Error: {Error}",
                    notification.Email,
                    result.Error);
            }
        }
    }
}

