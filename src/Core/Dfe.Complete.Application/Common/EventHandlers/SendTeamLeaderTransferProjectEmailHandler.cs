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
    /// Handles ProjectAssignedToRegionalTeamEvent for transfer projects.
    /// Sends notification emails to all active team leaders.
    /// Implements Ruby mailer: TeamLeaderMailer#new_transfer_project_created
    /// </summary>
    public class SendTeamLeaderTransferProjectEmailHandler(
        IEmailSender emailSender,
        IUserReadRepository userRepository,
        IProjectUrlBuilder projectUrlBuilder,
        ILogger<SendTeamLeaderTransferProjectEmailHandler> logger) : BaseEventHandler<ProjectAssignedToRegionalTeamEvent>(logger)
    {
        protected override async Task HandleEvent(
            ProjectAssignedToRegionalTeamEvent notification,
            CancellationToken cancellationToken)
        {
            // Only handle transfer projects
            if (notification.ProjectType != ProjectType.Transfer)
            {
                logger.LogDebug(
                    "Skipping email for non-transfer project {ProjectId} (Type: {ProjectType})",
                    notification.ProjectId,
                    notification.ProjectType);
                return;
            }

            logger.LogInformation(
                "Sending transfer project notification emails to team leaders for project {ProjectId}",
                notification.ProjectId);

            // Get all active team leaders (Ruby: User.team_leaders where active == true)
            var teamLeaders = await userRepository.GetActiveTeamLeadersAsync(cancellationToken);

            if (!teamLeaders.Any())
            {
                logger.LogWarning(
                    "No active team leaders found for transfer project {ProjectId}. No emails sent.",
                    notification.ProjectId);
                return;
            }

            var projectUrl = projectUrlBuilder.BuildProjectUrl(notification.ProjectReference);

            var emailsSent = 0;
            var emailsFailed = 0;

            foreach (var teamLeader in teamLeaders)
            {
                try
                {
                    var emailAddress = EmailAddress.Create(teamLeader.Email);

                    var emailMessage = new EmailMessage(
                        To: emailAddress,
                        TemplateKey: "NewTransferProjectCreated",
                        Personalisation: new Dictionary<string, string>
                        {
                            { "first_name", teamLeader.FirstName },
                            { "project_url", projectUrl }
                        });

                    var result = await emailSender.SendAsync(emailMessage, cancellationToken);

                    if (result.IsSuccess)
                    {
                        emailsSent++;
                        logger.LogInformation(
                            "Transfer project email sent to team leader {Email}. MessageId: {MessageId}",
                            teamLeader.Email,
                            result.Value?.ProviderMessageId);
                    }
                    else
                    {
                        emailsFailed++;
                        logger.LogError(
                            "Failed to send transfer project email to team leader {Email}. Error: {Error}",
                            teamLeader.Email,
                            result.Error);
                    }
                }
                catch (Exception ex)
                {
                    emailsFailed++;
                    logger.LogError(
                        ex,
                        "Exception sending transfer project email to team leader {Email}",
                        teamLeader.Email);
                }
            }

            logger.LogInformation(
                "Transfer project notification complete for {ProjectId}. Sent: {Sent}, Failed: {Failed}",
                notification.ProjectId,
                emailsSent,
                emailsFailed);
        }
    }
}

