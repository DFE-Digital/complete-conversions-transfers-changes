using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Validation;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Pages.Projects.ProjectHold;

[ExcludeFromCodeCoverage]
public class ConfirmResumeProjectModel(ISender sender, ILogger<ConfirmResumeProjectModel> logger, IProjectPermissionService projectPermissionService, ISignificantDateValidator dateValidator)
    : BaseProjectPageModel(sender, logger, projectPermissionService)
{
    private readonly ISignificantDateValidator _dateValidator = dateValidator;

    public override async Task<IActionResult> OnGetAsync()
    {
        await UpdateCurrentProject();
        await SetEstablishmentAsync();
        
        var validationResult = _dateValidator.ValidateSignificantDateInFuture(Project.SignificantDate);
        if (!validationResult.IsValid)
        {
            TempData.SetValidationNotification($"You cannot resume this project because the {(Project.Type == ProjectType.Conversion ? "conversion" : "transfer")} date is in the past. Change it to a future date and try again.");
            return Redirect(string.Format(RouteConstants.ProjectTaskListValidationError, ProjectId));
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await Sender.Send(new ClearProjectOnHoldCommand(new ProjectId(new Guid(ProjectId))), HttpContext.RequestAborted);
        if (result.IsSuccess)
        {
            TempData.SetNotification(
                NotificationType.Success,
                "Success",
                "The project was resumed."
            );

            return Redirect(string.Format(RouteConstants.Project, ProjectId));
        }

        return Page();
    }
}