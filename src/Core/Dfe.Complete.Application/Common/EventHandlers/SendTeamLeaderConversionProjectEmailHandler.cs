using Dfe.Complete.Application.Common.Constants;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Domain.Enums;
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
        ILogger<SendTeamLeaderConversionProjectEmailHandler> logger) : BaseTeamLeaderProjectEmailHandler<SendTeamLeaderConversionProjectEmailHandler>(emailSender, userRepository, projectUrlBuilder, logger)
    {
        protected override ProjectType ProjectType => ProjectType.Conversion;
        protected override string TemplateKey => EmailTemplateKeys.NewConversionProjectCreated;
        protected override string LogMessagePrefix => "Conversion";
    }
}

