using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.InternalContacts;

public class InternalContacts(ISender sender, ILogger<InternalContacts> logger) : ProjectLayoutModel(sender, logger, InternalContactsNavigation)
{
    private readonly ISender _sender = sender;
    public UserDto? UserAssignedTo { get; set; }
    public ProjectTeam? TeamAssignedTo { get; set; }
    public UserDto? UserAddedBy { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();
        if (Project.AssignedToId is not null)
        {
            var assignedToUserQuery = new GetUserByIdQuery(Project.AssignedToId);
            var assignedResult = await _sender.Send(assignedToUserQuery);
            if (assignedResult.IsSuccess)
            {
                UserAssignedTo = assignedResult.Value;
            }
            else
            {
                logger.LogError("Assigned to user id exists but user was not found by query - {id}", assignedToUserQuery.UserId.Value.ToString());
            }
        }
        TeamAssignedTo = Project.Team;
        var addedByUserQuery = new GetUserByIdQuery(Project.RegionalDeliveryOfficerId);
        var addedResult = await _sender.Send(addedByUserQuery);
        if (addedResult.IsSuccess)
        {
            UserAddedBy = addedResult.Value;
        }
        else
        {
            logger.LogError("Assigned to user id exists but user was not found by query - {id}", addedByUserQuery.UserId.Value.ToString());
        }
        return Page();
    }

}