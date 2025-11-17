using Dfe.Complete.Application.Common.Constants;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Application.Users.Queries.QueryFilters;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Common.EventHandlers
{
    /// <summary>
    /// Base class for team leader project email handlers.
    /// Contains shared logic for sending project notification emails to team leaders.
    /// </summary>
    /// <typeparam name="THandler">The concrete handler type for logging purposes.</typeparam>
    public abstract class BaseTeamLeaderProjectEmailHandler<THandler> : BaseEventHandler<ProjectAssignedToRegionalTeamEvent>
        where THandler : BaseTeamLeaderProjectEmailHandler<THandler>
    {
        protected readonly IEmailSender EmailSender;
        protected readonly IUserReadRepository UserRepository;
        protected readonly IProjectUrlBuilder ProjectUrlBuilder;
        protected readonly ILogger<BaseTeamLeaderProjectEmailHandler<THandler>> Logger;

        protected BaseTeamLeaderProjectEmailHandler(
            IEmailSender emailSender,
            IUserReadRepository userRepository,
            IProjectUrlBuilder projectUrlBuilder,
            ILogger<BaseTeamLeaderProjectEmailHandler<THandler>> logger) : base(logger)
        {
            EmailSender = emailSender;
            UserRepository = userRepository;
            ProjectUrlBuilder = projectUrlBuilder;
            Logger = logger;
        }

        /// <summary>
        /// Gets the project type this handler should process.
        /// </summary>
        protected abstract ProjectType ProjectType { get; }

        /// <summary>
        /// Gets the template key for the email.
        /// </summary>
        protected abstract string TemplateKey { get; }

        /// <summary>
        /// Gets the log message prefix (e.g., "Conversion" or "Transfer").
        /// </summary>
        protected abstract string LogMessagePrefix { get; }

        protected override async Task HandleEvent(
            ProjectAssignedToRegionalTeamEvent notification,
            CancellationToken cancellationToken)
        {
            // Only handle projects of the specified type
            if (notification.ProjectType != ProjectType)
            {
                Logger.LogDebug(
                    "Skipping email for non-{ProjectType} project {ProjectId} (Type: {ActualType})",
                    ProjectType,
                    notification.ProjectId,
                    notification.ProjectType);
                return;
            }

            Logger.LogInformation(
                "Sending {LogPrefix} project notification emails to team leaders for project {ProjectId}",
                LogMessagePrefix,
                notification.ProjectId);

            // Get all active team leaders
            var teamLeadersQuery = new ActiveTeamLeadersQuery().Apply(UserRepository.Users);
            var teamLeaders = await teamLeadersQuery.ToListAsync(cancellationToken);

            if (teamLeaders.Count == 0)
            {
                Logger.LogWarning(
                    "No active team leaders found for {LogPrefix} project {ProjectId}. No emails sent.",
                    LogMessagePrefix,
                    notification.ProjectId);
                return;
            }

            var projectUrl = ProjectUrlBuilder.BuildProjectUrl(notification.ProjectReference);

            var emailsSent = 0;
            var emailsFailed = 0;

            foreach (var teamLeader in teamLeaders)
            {
                try
                {
                    // Guard: Skip if email or first name is null
                    if (string.IsNullOrWhiteSpace(teamLeader.Email))
                    {
                        Logger.LogWarning(
                            "Skipping email for team leader {UserId} - email is null or empty for project {ProjectId}",
                            teamLeader.Id,
                            notification.ProjectId);
                        emailsFailed++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(teamLeader.FirstName))
                    {
                        Logger.LogWarning(
                            "Skipping email for team leader {UserId} - first name is null or empty for project {ProjectId}",
                            teamLeader.Id,
                            notification.ProjectId);
                        emailsFailed++;
                        continue;
                    }

                    var emailAddress = EmailAddress.Create(teamLeader.Email);

                    var emailMessage = new EmailMessage(
                        To: emailAddress,
                        TemplateKey: TemplateKey,
                        Personalisation: new Dictionary<string, string>
                        {
                            { EmailPersonalisationKeys.FirstName, teamLeader.FirstName },
                            { EmailPersonalisationKeys.ProjectUrl, projectUrl }
                        });

                    var result = await EmailSender.SendAsync(emailMessage, cancellationToken);

                    if (result.IsSuccess)
                    {
                        emailsSent++;
                        Logger.LogInformation(
                            "{LogPrefix} project email sent to team leader {UserId}. MessageId: {MessageId}",
                            LogMessagePrefix,
                            teamLeader.Id,
                            result.Value?.ProviderMessageId);
                    }
                    else
                    {
                        emailsFailed++;
                        Logger.LogError(
                            "Failed to send {LogPrefix} project email to team leader {UserId}. Error: {Error}",
                            LogMessagePrefix,
                            teamLeader.Id,
                            result.Error);
                    }
                }
                catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is KeyNotFoundException)
                {
                    // Catch specific exceptions that can occur during email creation/sending
                    // These are expected and should be logged but not rethrown
                    emailsFailed++;
                    Logger.LogError(
                        ex,
                        "Exception sending {LogPrefix} project email to team leader {UserId}",
                        LogMessagePrefix,
                        teamLeader.Id);
                }
            }

            Logger.LogInformation(
                "{LogPrefix} project notification complete for {ProjectId}. Sent: {Sent}, Failed: {Failed}",
                LogMessagePrefix,
                notification.ProjectId,
                emailsSent,
                emailsFailed);
        }
    }
}

