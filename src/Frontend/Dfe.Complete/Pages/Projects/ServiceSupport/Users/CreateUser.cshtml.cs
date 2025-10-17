using Dfe.Complete.Application.Users.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using ValidationConstants = Dfe.Complete.Constants.ValidationConstants;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.Users
{
    public class CreateUserModel(ISender sender, IErrorService errorService) : UserDetailsModel(sender, errorService)
    {
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }

            var response = await Sender.Send(new CreateUserCommand(FirstName, LastName, Email, Team));

            if (response.IsSuccess)
            {
                TempData.SetNotification(
                    NotificationType.Success,
                    "Success",
                    $"User {Email} added successfully"
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
}
