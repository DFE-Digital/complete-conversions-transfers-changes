using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.InternalContacts;

public class EditAddedByUser(ISender sender, ErrorService errorService, ILogger<InternalContacts> logger) : BaseProjectPageModel(sender)
{
    private readonly ISender _sender = sender;

    [BindProperty]
    [InternalEmail]
    public string Email { get; set; } = default!;

    public UserDto AddedByUser { get; set; } = default!;
    
    public override async Task<IActionResult> OnGet()
    {
        await base.OnGet();
        var addedByUserQuery = new GetUserByIdQuery(Project.RegionalDeliveryOfficerId);
        var addedbyResult = await _sender.Send(addedByUserQuery);
        if (addedbyResult is { IsSuccess: true, Value: not null })
        {
            AddedByUser = addedbyResult.Value;
            Email = addedbyResult.Value.Email ?? "";
        }
        else
        {
            logger.LogError("Added by user id exists but user was not found by query - {UserId}", addedByUserQuery.UserId.Value.ToString());
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
        
        var addedByUserQuery = new GetUserByEmailQuery(Email);
        var addedBySearchResult = await _sender.Send(addedByUserQuery);
        
        if (addedBySearchResult is { IsSuccess: true, Value.AssignToProject: true })
        {
            var updateRequest = new UpdateRegionalDeliveryOfficerCommand(Project.Id, addedBySearchResult.Value.Id);
            await _sender.Send(updateRequest);
            TempData.SetNotification(NotificationType.Success, "Success", "Project has been updated successfully");
            return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectInternalContacts));
        }

        logger.LogError("Email not found or not assignable - {Email}", addedByUserQuery.Email);
        ModelState.AddModelError("Email", "Email is not assignable");
        return await OnGet();
    }
}