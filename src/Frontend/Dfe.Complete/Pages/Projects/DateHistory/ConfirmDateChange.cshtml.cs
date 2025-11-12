using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.DateHistory
{
    public class ConfirmDateChangeModel(ISender sender, ILogger<ConfirmDateChangeModel> logger) : ProjectLayoutModel(sender, logger, ConversionDateHistoryNavigation)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (!SigDateHelper.CanEditSignificantDate(Project, User, CurrentUserTeam))
            {
                TempData.SetNotification(
                    NotificationType.Error,
                    "Important",
                    "You are not authorised to perform this action."
                );
                return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectTaskList));
            }

            return Page();
        }
    }
}