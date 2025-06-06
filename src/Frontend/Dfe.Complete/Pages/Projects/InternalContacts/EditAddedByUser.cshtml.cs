using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.InternalContacts;

public class EditAddedByUser(ISender sender, ErrorService errorService, ILogger<InternalContacts> logger) : BaseProjectPageModel(sender)
{
    private readonly ISender _sender = sender;

    [BindProperty]
    [EmailAddress]
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
            logger.LogError("Added by user id exists but user was not found by query - {id}", addedByUserQuery.UserId.Value.ToString());
        }
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return await OnGet();
        }
        return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectInternalContacts));
    }
}