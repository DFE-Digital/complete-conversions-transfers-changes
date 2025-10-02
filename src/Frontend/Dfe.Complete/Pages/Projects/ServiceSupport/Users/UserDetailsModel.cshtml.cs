using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.Users
{
    public class UserDetailsModel(ISender sender, IErrorService errorService) : ServiceSupportModel(UsersNavigation)
    {
        protected ISender Sender { get; } = sender;
        protected IErrorService ErrorService { get; } = errorService;

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
    }
}
