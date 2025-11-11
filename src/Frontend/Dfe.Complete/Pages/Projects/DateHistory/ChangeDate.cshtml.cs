using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.DateHistory
{
    public class ChangeDateProjectModel(ISender sender, IErrorService errorService, ILogger<ChangeDateProjectModel> logger) : ProjectLayoutModel(sender, logger, ConversionDateHistoryNavigation)
    {
        [BindProperty]
        [Required(ErrorMessage = "Enter a valid month and year for the revised date, like 9 2024")]
        [Display(Name = "Significant Date")]
        public DateOnly? SignificantDate { get; set; }

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

        public async Task<IActionResult> OnPost()
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

            if (SignificantDate?.ToDateTime(new TimeOnly()) < DateTime.Today)
            {
                ModelState.AddModelError(nameof(SignificantDate), "The Significant date cannot be in the past");
            }

            if (SignificantDate == Project.SignificantDate)
            {
                ModelState.AddModelError(nameof(SignificantDate), "The new date cannot be the same as the current date. Check you have entered the correct date.");
            }

            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            }

            TempData["SignificantDate"] = SignificantDate?.ToString();

            return Redirect(FormatRouteWithProjectId(RouteConstants.ChangeProjectDateHistoryReason));
        }
    };
}
