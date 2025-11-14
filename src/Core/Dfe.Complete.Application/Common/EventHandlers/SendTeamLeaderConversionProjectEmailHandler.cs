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
    /// Handles ProjectAssignedToRegionalTeamEvent for conversion projects.
    /// Sends notification emails to all active team leaders.
    /// Implements Ruby mailer: TeamLeaderMailer#new_conversion_project_created
    /// </summary>
    public class SendTeamLeaderConversionProjectEmailHandler(
        IEmailSender emailSender,
        IUserReadRepository userRepository,
        IProjectUrlBuilder projectUrlBuilder,
        ILogger<SendTeamLeaderConversionProjectEmailHandler> logger) : BaseEventHandler<ProjectAssignedToRegionalTeamEvent>(logger)
    {
        protected override async Task HandleEvent(
            ProjectAssignedToRegionalTeamEvent notification,
            CancellationToken cancellationToken)
        {
            // Only handle conversion projects
            if (notification.ProjectType != ProjectType.Conversion)
            {
                logger.LogDebug(
                    "Skipping email for non-conversion project {ProjectId} (Type: {ProjectType})",
                    notification.ProjectId,
                    notification.ProjectType);
                return;
            }

            logger.LogInformation(
                "Sending conversion project notification emails to team leaders for project {ProjectId}",
                notification.ProjectId);

            // Get all active team leaders (Ruby: User.team_leaders where active == true)
            var teamLeaders = await userRepository.GetActiveTeamLeadersAsync(cancellationToken);

            if (!teamLeaders.Any())
            {
                logger.LogWarning(
                    "No active team leaders found for conversion project {ProjectId}. No emails sent.",
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
                        TemplateKey: "NewConversionProjectCreated",
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
                            "Conversion project email sent to team leader {Email}. MessageId: {MessageId}",
                            teamLeader.Email,
                            result.Value?.ProviderMessageId);
                    }
                    else
                    {
                        emailsFailed++;
                        logger.LogError(
                            "Failed to send conversion project email to team leader {Email}. Error: {Error}",
                            teamLeader.Email,
                            result.Error);
                    }
                }
                catch (Exception ex)
                {
                    emailsFailed++;
                    logger.LogError(
                        ex,
                        "Exception sending conversion project email to team leader {Email}",
                        teamLeader.Email);
                }
            }

            logger.LogInformation(
                "Conversion project notification complete for {ProjectId}. Sent: {Sent}, Failed: {Failed}",
                notification.ProjectId,
                emailsSent,
                emailsFailed);
        }
    }
}

