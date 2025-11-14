using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Common.EventHandlers
{
    /// <summary>
    /// Handles ProjectAssignedToUserEvent and sends notification email to assigned caseworker.
    /// Implements Ruby mailer: AssignedToMailer#assigned_notification
    /// </summary>
    public class SendAssignedNotificationEmailHandler(
        IEmailSender emailSender,
        IUserReadRepository userRepository,
        IProjectUrlBuilder projectUrlBuilder,
        ILogger<SendAssignedNotificationEmailHandler> logger) : BaseEventHandler<ProjectAssignedToUserEvent>(logger)
    {
        protected override async Task HandleEvent(
            ProjectAssignedToUserEvent notification,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Sending project assignment notification email to user {UserId} for project {ProjectId} (Type: {ProjectType})",
                notification.AssignedToUserId,
                notification.ProjectId,
                notification.ProjectType);

            // Guard: Check if user exists
            var user = await userRepository.GetByIdAsync(notification.AssignedToUserId, cancellationToken);
            
            if (user == null)
            {
                logger.LogWarning(
                    "User {UserId} not found. Cannot send assignment notification for project {ProjectId}.",
                    notification.AssignedToUserId,
                    notification.ProjectId);
                return;
            }

            var projectUrl = projectUrlBuilder.BuildProjectUrl(notification.ProjectReference);

            var emailAddress = EmailAddress.Create(notification.AssignedToEmail);

            // Select template based on project type
            var templateKey = notification.ProjectType switch
            {
                ProjectType.Conversion => "AssignedNotificationConversion",
                ProjectType.Transfer => "AssignedNotificationTransfer",
                _ => throw new InvalidOperationException($"Unsupported project type for assignment notification: {notification.ProjectType}")
            };

            logger.LogDebug(
                "Using template {TemplateKey} for {ProjectType} project {ProjectId}",
                templateKey,
                notification.ProjectType,
                notification.ProjectId);

            var emailMessage = new EmailMessage(
                To: emailAddress,
                TemplateKey: templateKey,
                Personalisation: new Dictionary<string, string>
                {
                    { "first_name", notification.AssignedToFirstName },
                    { "project_url", projectUrl }
                });

            var result = await emailSender.SendAsync(emailMessage, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Assignment notification sent to {Email} using template {TemplateKey}. MessageId: {MessageId}",
                    notification.AssignedToEmail,
                    templateKey,
                    result.Value?.ProviderMessageId);
            }
            else
            {
                logger.LogError(
                    "Failed to send assignment notification to {Email} using template {TemplateKey}. Error: {Error}",
                    notification.AssignedToEmail,
                    templateKey,
                    result.Error);
            }
        }
    }
}

