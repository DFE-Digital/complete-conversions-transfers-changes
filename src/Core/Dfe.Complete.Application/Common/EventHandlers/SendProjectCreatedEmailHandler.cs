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
    /// Handles ProjectCreatedEvent when project is created and assigned to regional team.
    /// Sends notification emails to all active team leaders.
    /// Implements AC1: NEW Project Received from Prepare - Trigger Notification Email
    /// </summary>
    public class SendProjectCreatedEmailHandler(
        IEmailSender emailSender,
        IUserReadRepository userRepository,
        IProjectUrlBuilder projectUrlBuilder,
        ILogger<SendProjectCreatedEmailHandler> logger) : BaseEventHandler<ProjectCreatedEvent>(logger)
    {
        protected override async Task HandleEvent(
            ProjectCreatedEvent notification,
            CancellationToken cancellationToken)
        {
            var project = notification.Project;

            // Only handle projects assigned to regional caseworker team
            if (project.Team != ProjectTeam.RegionalCaseWorkerServices)
            {
                logger.LogDebug(
                    "Skipping email for project {ProjectId} assigned to team {Team} (not RegionalCaseWorkerServices)",
                    project.Id,
                    project.Team);
                return;
            }

            logger.LogInformation(
                "Sending new project notification emails to team leaders for project {ProjectId} (Type: {ProjectType})",
                project.Id,
                project.Type);

            // Get all active team leaders
            var teamLeaders = await userRepository.GetActiveTeamLeadersAsync(cancellationToken);

            if (!teamLeaders.Any())
            {
                logger.LogWarning(
                    "No active team leaders found for project {ProjectId}. No emails sent.",
                    project.Id);
                return;
            }

            // Determine template based on project type
            var templateKey = project.Type switch
            {
                ProjectType.Conversion => "NewConversionProjectCreated",
                ProjectType.Transfer => "NewTransferProjectCreated",
                _ => throw new InvalidOperationException($"Unsupported project type: {project.Type}")
            };

            var projectUrl = projectUrlBuilder.BuildProjectUrl(project.Id.Value.ToString());

            var emailsSent = 0;
            var emailsFailed = 0;

            foreach (var teamLeader in teamLeaders)
            {
                try
                {
                    var emailAddress = EmailAddress.Create(teamLeader.Email);

                    var emailMessage = new EmailMessage(
                        To: emailAddress,
                        TemplateKey: templateKey,
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
                            "New project email sent to team leader {Email} for project {ProjectId}. MessageId: {MessageId}",
                            teamLeader.Email,
                            project.Id,
                            result.Value?.ProviderMessageId);
                    }
                    else
                    {
                        emailsFailed++;
                        logger.LogError(
                            "Failed to send new project email to team leader {Email} for project {ProjectId}. Error: {Error}",
                            teamLeader.Email,
                            project.Id,
                            result.Error);
                    }
                }
                catch (Exception ex)
                {
                    emailsFailed++;
                    logger.LogError(
                        ex,
                        "Exception sending new project email to team leader {Email} for project {ProjectId}",
                        teamLeader.Email,
                        project.Id);
                }
            }

            logger.LogInformation(
                "New project notification complete for {ProjectId}. Sent: {Sent}, Failed: {Failed}",
                project.Id,
                emailsSent,
                emailsFailed);
        }
    }
}

