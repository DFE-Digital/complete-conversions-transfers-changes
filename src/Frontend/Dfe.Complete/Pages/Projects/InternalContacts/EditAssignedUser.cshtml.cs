using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.InternalContacts;

public class EditAssignedUser(ISender sender, ErrorService errorService, ILogger<InternalContacts> logger)
    : BaseProjectPageModel(sender)
{
    private readonly ISender _sender = sender;

    [BindProperty] [InternalEmail] public string Email { get; set; } = default!;

    public UserDto AssignedUser { get; set; } = default!;

    public override async Task<IActionResult> OnGet()
    {
        await base.OnGet();
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

    public async Task<IActionResult> OnPost()
    {
        await UpdateCurrentProject();

        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return await OnGet();
        }

        var assignedToUserQuery = new GetUserByEmailQuery(Email);
        var assignedResult = await _sender.Send(assignedToUserQuery);
        
        if (assignedResult is { IsSuccess: true, Value.AssignToProject: true })
        {
            try
            {
                var updateRequest = new UpdateAssignedUserCommand(Project.Id, assignedResult.Value.Id);
                await _sender.Send(updateRequest);
                TempData.SetNotification(NotificationType.Success, "Success", "Project has been assigned successfully");
                return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectInternalContacts));
            }
            catch (NotFoundException notFoundException)
            {
                logger.LogInformation(notFoundException, notFoundException.Message, notFoundException.InnerException);

                ModelState.AddModelError(notFoundException.Field ?? "NotFound", notFoundException.Message);

                errorService.AddErrors(ModelState);

                return await OnGet();
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, "Error occurred while updating the assigned user.");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return await OnGet();
            }
        }

        logger.LogError("Email not found or not assignable - {Email}", assignedToUserQuery.Email);
        ModelState.AddModelError("Email", "Email is not assignable");
        return await OnGet();
    }
}