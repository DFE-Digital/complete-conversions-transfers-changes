using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations; 

namespace Dfe.Complete.Pages.Projects.List.HandoverProjects
{
    [Authorize(policy: UserPolicyConstants.CanViewAllProjectsHandover)]
    public class NewHandoverProjectModel(ISender sender, ErrorService errorService) : BaseProjectPageModel(sender)
    { 
        [BindProperty]
        public ProjectType? Type { get; set; }

        [BindProperty]
        [SharePointLink]
        [Required]
        [Display(Name = ValidationConstants.SchoolOrAcademySharePointLink)]
        public required string SchoolSharePointLink { get; set; }

        [BindProperty]
        [SharePointLink]
        [Required]
        [Display(Name = ValidationConstants.IncomingSharePointLink)]
        public required string IncomingTrustSharePointLink { get; set; }

        [BindProperty]
        [SharePointLink]
        [Display(Name = ValidationConstants.OutgoingSharePointLink)]
        public string? OutgoingTrustSharePointLink { get; set; }

        [BindProperty]
        [Required(ErrorMessage = ValidationConstants.AssignedToRegionalCaseworkerTeam)]
        [Display(Name = "Is Handing To RCS")]
        public required bool? AssignedToRegionalCaseworkerTeam { get; set; }

        [BindProperty]
        public string? HandoverComments { get; set; }

        [BindProperty]
        public bool? TwoRequiresImprovement { get; set; }

        public bool DisplayConfirmation { get; set; }

        [BindProperty]
        public int UrnId { get; set; }
        [BindProperty]
        public string? EstablishmentName { get; set; } 

        public override async Task<IActionResult> OnGetAsync()
        {
            await UpdateCurrentProject();
            Type = Project.Type;
            UrnId = Project.Urn.Value;
            EstablishmentName = Project.EstablishmentName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            ValidateOptionalPropertiesOnMeetingCertainCriteria(); 
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            } 
            var userId = User.GetUserId();
            await SetCurrentUserTeamAsync();

            await sender.Send(new UpdateHandoverProjectCommand(new ProjectId(new Guid(ProjectId)), SchoolSharePointLink, IncomingTrustSharePointLink, OutgoingTrustSharePointLink,
                AssignedToRegionalCaseworkerTeam!.Value, HandoverComments, userId, CurrentUserTeam, TwoRequiresImprovement), cancellationToken);
            DisplayConfirmation = true;

            return Page();
        }

        private void ValidateOptionalPropertiesOnMeetingCertainCriteria()
        {
            if (AssignedToRegionalCaseworkerTeam.HasValue && AssignedToRegionalCaseworkerTeam.Value && string.IsNullOrWhiteSpace(HandoverComments))
            {
                ModelState.AddModelError(nameof(HandoverComments), ValidationConstants.HandoverNotes);
            }
            if (Type == ProjectType.Transfer && string.IsNullOrWhiteSpace(OutgoingTrustSharePointLink))
            {
                ModelState.AddModelError(nameof(OutgoingTrustSharePointLink), ValidationConstants.OutgoingSharePointLink);
            }

            if (Type == ProjectType.Conversion && TwoRequiresImprovement == null)
            {
                ModelState.AddModelError(nameof(TwoRequiresImprovement), ValidationConstants.TwoRequiresImprovement);
            }
        }
    }

}
