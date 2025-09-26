using Dfe.Complete.Application.Users.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.Users
{
    public class CreateUserModel(ISender sender, IErrorService errorService) : ServiceSupportModel(UsersNavigation)
    {
        [BindProperty(Name = nameof(FirstName))]
        [Required(ErrorMessage = "Enter a first name")]
        public string FirstName { get; set; } = null!;

        [BindProperty(Name = nameof(LastName))]
        [Required(ErrorMessage = "Enter a last name")]
        public string LastName { get; set; } = null!;

        [BindProperty(Name = nameof(Email))]
        [InternalEmail]
        [Required(ErrorMessage = "Enter an email")]
        public string Email { get; set; } = null!;

        [BindProperty(Name = nameof(Team))]
        [Required(ErrorMessage = "Choose a team")]
        public ProjectTeam Team { get; set; }

        public List<ProjectTeam> AvailableTeams { get; set; } = EnumExtensions.ToList<ProjectTeam>();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }

            var response = await sender.Send(new CreateUserCommand(FirstName, LastName, Email, Team));

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
                errorService.AddError(nameof(Email), ValidationConstants.AlreadyBeenTaken);
                ModelState.AddModelError(nameof(Email), ValidationConstants.AlreadyBeenTaken);
            }
            return Page();
        }
    }
}
