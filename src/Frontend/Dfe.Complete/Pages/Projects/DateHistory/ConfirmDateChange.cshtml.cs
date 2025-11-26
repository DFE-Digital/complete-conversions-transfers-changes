using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.DateHistory
{
    public class ConfirmDateChangeModel(ISender sender, ILogger<ConfirmDateChangeModel> logger, IProjectPermissionService projectPermissionService) : ProjectLayoutModel(sender, logger, projectPermissionService, ConversionDateHistoryNavigation)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (!CanEditSignificantDate)
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