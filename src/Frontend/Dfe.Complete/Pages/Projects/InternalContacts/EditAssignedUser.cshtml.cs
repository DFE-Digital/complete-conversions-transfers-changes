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
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.InternalContacts;

[Authorize(Policy = UserPolicyConstants.CanManageInternalContacts)]
public class EditAssignedUser(ISender sender, IErrorService errorService, ILogger<InternalContacts> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectPageModel(sender, logger, projectPermissionService)
{
    private readonly ISender _sender = sender;

    [BindProperty][InternalEmail] public string Email { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public UserDto AssignedUser { get; set; } = default!;

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();
        if (Project.AssignedToId is null) return Page();

        var assignedToUserQuery = new GetUserByIdQuery(Project.AssignedToId);
        var assignedResult = await _sender.Send(assignedToUserQuery);
        if (assignedResult is { IsSuccess: true, Value: not null })
        {
            AssignedUser = assignedResult.Value;
            Email = assignedResult.Value.Email ?? "";
        }
        else
        {
            logger.LogInformation("Assigned to user id exists but user was not found by query - {Id}",
                assignedToUserQuery.UserId.Value.ToString());
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

        var assignedToUserQuery = new GetUserByEmailQuery(rawEmail);
        var assignedResult = await _sender.Send(assignedToUserQuery);

        if (assignedResult is { IsSuccess: true, Value.IsAssignableToProject: true })
        {
            try
            {
                var updateRequest = new UpdateAssignedUserCommand(Project.Id, assignedResult.Value.Id);
                await _sender.Send(updateRequest);
                TempData.SetNotification(NotificationType.Success, "Success", "Project has been assigned successfully");

                return Redirect(DetermineReturnRoute());
            }
            catch (NotFoundException notFoundException)
            {
                logger.LogInformation(notFoundException, notFoundException.Message, notFoundException.InnerException);

                ModelState.AddModelError(notFoundException.Field ?? "NotFound", notFoundException.Message);

                errorService.AddErrors(ModelState);

                return await OnGetAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, "Error occurred while updating the assigned user.");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return await OnGetAsync();
            }
        }

        logger.LogError("Email not found or not assignable - {Email}", assignedToUserQuery.Email);
        ModelState.AddModelError("Email", "Email is not assignable");

        await base.OnGetAsync();
        return Page();
    }

    public string DetermineReturnRoute()
    {
        var returnRoute = ReturnUrl == "unassigned"
            ? FormatRouteWithProjectId(RouteConstants.TeamProjectsUnassigned)
            : FormatRouteWithProjectId(RouteConstants.ProjectInternalContacts);

        return returnRoute;
    }
}