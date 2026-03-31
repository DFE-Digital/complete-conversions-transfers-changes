using Dfe.Complete.Application.Validation;
using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.DateHistory;

public class ChangeDateProjectModel(ISender sender, IErrorService errorService, ILogger<ChangeDateProjectModel> logger, IProjectPermissionService projectPermissionService, ISignificantDateValidator dateValidator) : ProjectLayoutModel(sender, logger, projectPermissionService, ConversionDateHistoryNavigation)
{
    private readonly ISignificantDateValidator _dateValidator = dateValidator;

    [BindProperty]
    [Required(ErrorMessage = "Enter a valid month and year for the revised date, like 9 2024")]
    [Display(Name = "Significant Date")]
    public DateOnly? SignificantDate { get; set; }

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

    public async Task<IActionResult> OnPost()
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

        var validationResult = _dateValidator.ValidateSignificantDate(SignificantDate, Project);
        if (!validationResult.IsValid)
        {
            ModelState.AddModelError(nameof(SignificantDate), validationResult.ErrorMessage!);
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
