using Dfe.Complete.Application.Common.Constants;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Domain.Enums;
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
        ILogger<SendTeamLeaderTransferProjectEmailHandler> logger) : BaseTeamLeaderProjectEmailHandler<SendTeamLeaderTransferProjectEmailHandler>(emailSender, userRepository, projectUrlBuilder, logger)
    {
        protected override ProjectType ProjectType => ProjectType.Transfer;
        protected override string TemplateKey => EmailTemplateKeys.NewTransferProjectCreated;
        protected override string LogMessagePrefix => "Transfer";
    }
}

