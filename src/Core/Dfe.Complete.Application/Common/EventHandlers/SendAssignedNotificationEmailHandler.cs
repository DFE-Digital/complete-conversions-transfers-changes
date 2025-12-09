using Dfe.Complete.Application.Common.Constants;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Application.Users.Queries.QueryFilters;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
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
            var userQuery = new UserIdQuery(notification.AssignedToUserId).Apply(userRepository.Users);
            var user = await userQuery.FirstOrDefaultAsync(cancellationToken);
            
            if (user == null)
            {
                logger.LogWarning(
                    "User {UserId} not found. Cannot send assignment notification for project {ProjectId}.",
                    notification.AssignedToUserId,
                    notification.ProjectId);
                return;
            }

            // Use user's email from database, fallback to notification email if user email is null/empty
            var emailToUse = !string.IsNullOrWhiteSpace(user.Email) 
                ? user.Email 
                : notification.AssignedToEmail;

            // Guard: Validate email before attempting to send
            if (string.IsNullOrWhiteSpace(emailToUse))
            {
                logger.LogWarning(
                    "User {UserId} has no email address. Cannot send assignment notification for project {ProjectId}.",
                    notification.AssignedToUserId,
                    notification.ProjectId);
                return;
            }

            EmailAddress emailAddress;
            try
            {
                emailAddress = EmailAddress.Create(emailToUse);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(
                    ex,
                    "Invalid email address format for user {UserId}: {Email}. Cannot send assignment notification for project {ProjectId}.",
                    notification.AssignedToUserId,
                    emailToUse,
                    notification.ProjectId);
                return;
            }

            var projectUrl = projectUrlBuilder.BuildProjectUrl(notification.ProjectReference);

            // Select template based on project type
            var templateKey = notification.ProjectType switch
            {
                ProjectType.Conversion => EmailTemplateKeys.AssignedNotificationConversion,
                ProjectType.Transfer => EmailTemplateKeys.AssignedNotificationTransfer,
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
                    { EmailPersonalisationKeys.FirstName, notification.AssignedToFirstName },
                    { EmailPersonalisationKeys.ProjectUrl, projectUrl }
                });

            var result = await emailSender.SendAsync(emailMessage, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Assignment notification sent to user {UserId} using template {TemplateKey}. MessageId: {MessageId}",
                    notification.AssignedToUserId,
                    templateKey,
                    result.Value?.ProviderMessageId);
            }
            else
            {
                logger.LogError(
                    "Failed to send assignment notification to user {UserId} using template {TemplateKey}. Error: {Error}",
                    notification.AssignedToUserId,
                    templateKey,
                    result.Error);
            }
        }
    }
}

