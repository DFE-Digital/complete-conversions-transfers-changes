using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.InternalContacts;

[Authorize(Policy = UserPolicyConstants.CanEditAddedByUser)]
public class EditAddedByUser(ISender sender, IErrorService errorService, ILogger<InternalContacts> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectPageModel(sender, logger, projectPermissionService)
{
    private readonly ISender _sender = sender;

    [BindProperty][InternalEmail] public string Email { get; set; } = default!;

    public UserDto AddedByUser { get; set; } = default!;

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();
        var addedByUserQuery = new GetUserByIdQuery(Project.RegionalDeliveryOfficerId);
        var addedbyResult = await _sender.Send(addedByUserQuery);
        if (addedbyResult is { IsSuccess: true, Value: not null })
        {
            AddedByUser = addedbyResult.Value;
            Email = addedbyResult.Value.Email ?? "";
        }
        else
        {
            logger.LogInformation("Added by user id exists but user was not found by query - {UserId}",
                addedByUserQuery.UserId.Value.ToString());
        }

        return Page();
    }

    public async Task<IActionResult> OnPost(string rawEmail)
    {
        await UpdateCurrentProject();

        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return await OnGetAsync();
        }

        var addedByUserQuery = new GetUserByEmailQuery(rawEmail);
        var addedBySearchResult = await _sender.Send(addedByUserQuery);

        if (addedBySearchResult is { IsSuccess: true, Value.IsAssignableToProject: true })
        {
            var updateRequest = new UpdateRegionalDeliveryOfficerCommand(Project.Id, addedBySearchResult.Value.Id);
            var result = await _sender.Send(updateRequest);
            if (result.IsSuccess)
            {
                TempData.SetNotification(NotificationType.Success, "Success", "Project has been updated successfully");
                return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectInternalContacts));
            }
            logger.LogInformation("An error occured when trying up update the assigned user.");
            ModelState.AddModelError("Misc", "An error occured when trying up update.");
            return await OnGetAsync();
        }

        logger.LogInformation("Email not found or not assignable - {Email}", addedByUserQuery.Email);
        ModelState.AddModelError("Email", "Email is not assignable");

        await base.OnGetAsync();
        return Page();
    }
}