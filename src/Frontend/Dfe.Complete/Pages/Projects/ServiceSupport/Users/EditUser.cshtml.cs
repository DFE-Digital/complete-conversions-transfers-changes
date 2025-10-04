using Dfe.Complete.Application.Users.Commands;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using ValidationConstants = Dfe.Complete.Constants.ValidationConstants;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.Users;

public class EditUserModel(ISender sender, IErrorService errorService, ILogger<EditUserModel> logger) : UserDetailsModel(sender, errorService)
{
    [BindProperty(SupportsGet = true, Name = nameof(UserId))]
    public required string UserId { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var success = Guid.TryParse(UserId, out var guid);

        if (!success)
        {
            logger.LogWarning(string.Format(ErrorMessagesConstants.InvalidGuid, UserId), UserId);
            throw new Exception(string.Format(ErrorMessagesConstants.InvalidGuid, UserId));
        }

        var userGuid = new UserId(guid);
        var userResponse = await Sender.Send(new GetUserByIdQuery(userGuid));

        if (userResponse.IsSuccess && userResponse.Value != null)
        {
            var user = userResponse.Value;
            FirstName = user.FirstName ?? string.Empty;
            LastName = user.LastName ?? string.Empty;
            Email = user.Email ?? string.Empty;

            if (user.Team != null)
            {
                var teamResult = user.Team.FromDescriptionValue<ProjectTeam>();
                if (teamResult.HasValue)
                {
                    ProjectTeam team = teamResult.Value;
                    Team = team;
                }
            }
        }
        else
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorService.AddErrors(ModelState.Keys, ModelState);
            return Page();
        }

        var response = await Sender.Send(new UpdateUserCommand(new UserId(new Guid(UserId)), FirstName, LastName, Email, Team));

        if (response.IsSuccess)
        {
            TempData.SetNotification(
                NotificationType.Success,
                "Success",
                $"User {Email} updated successfully"
            );

            return Redirect(RouteConstants.ServiceSupportUsers);
        }
        else if (response.Error == string.Format(ErrorMessagesConstants.AlreadyExistsUserWithCode, Email))
        {
            ErrorService.AddError(nameof(Email), ValidationConstants.AlreadyBeenTaken);
            ModelState.AddModelError(nameof(Email), ValidationConstants.AlreadyBeenTaken);
        }
        return Page();
    }
}
