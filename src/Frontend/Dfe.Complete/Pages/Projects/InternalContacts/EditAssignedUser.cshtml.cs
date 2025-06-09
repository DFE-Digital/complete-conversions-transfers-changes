using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.InternalContacts;

public class EditAssignedUser(ISender sender, ErrorService errorService, ILogger<InternalContacts> logger) : BaseProjectPageModel(sender)
{
    private readonly ISender _sender = sender;

    [BindProperty]
    [EmailAddress]
    public string Email { get; set; } = default!;

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
            logger.LogError("Assigned to user id exists but user was not found by query - {Id}", assignedToUserQuery.UserId.Value.ToString());
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
        
        if (assignedResult is { IsSuccess: true, Value: not null  })
        {
            var updateRequest = new UpdateAssignedUserCommand(Project.Urn, assignedResult.Value.Id);
            await sender.Send(updateRequest);
            return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectInternalContacts));
        }

        logger.LogError("Email not found - {Email}", assignedToUserQuery.Email);
        return await OnGet();

    }
}