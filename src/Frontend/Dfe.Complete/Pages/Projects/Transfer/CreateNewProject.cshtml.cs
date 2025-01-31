using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Validators;
using Dfe.Complete.Services;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Extensions;

namespace Dfe.Complete.Pages.Projects.Transfer
{
    [Authorize(policy: "CanCreateProjects")]
    public class CreateNewProjectModel(ISender sender, IErrorService errorService, ITrustsClient trustsClient) : PageModel
    {
        [BindProperty]
        [Required]
        [Urn]
        [Display(Name = "Urn")]
        public string URN { get; set; }

        [BindProperty]
        [Ukprn(comparisonProperty: $"{nameof(IncomingUKPRN)}")]
        [Display(Name = "OutgoingUKPRN")]
        public string OutgoingUKPRN { get; set; }

        [BindProperty]
        [Ukprn(comparisonProperty: $"{nameof(OutgoingUKPRN)}")]
        [Display(Name = "IncomingUKPRN")]
        public string IncomingUKPRN { get; set; }

        [BindProperty]
        [GroupReferenceNumber(ShouldMatchWithTrustUkprn: true, nameof(IncomingUKPRN))]
        [Display(Name = "Group Reference Number")]
        public string? GroupReferenceNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Enter a date for the Advisory Board Date, like 1 4 2023")]
        [Display(Name = "Advisory Board Date")]
        public DateTime? AdvisoryBoardDate { get; set; }

        [BindProperty] 
        public string? AdvisoryBoardConditions { get; set; }

        [BindProperty]
        [SharePointLink]
        [Required(ErrorMessage = "Enter a school SharePoint link")]
        [Display(Name = "School or academy SharePoint link")]
        public string AcademySharePointLink { get; set; }

        [BindProperty]
        [SharePointLink]
        [Required(ErrorMessage = "Enter an incoming trust SharePoint link")]
        [Display(Name = "Incoming trust SharePoint link")]
        public string IncomingTrustSharePointLink { get; set; }

        [BindProperty]
        [SharePointLink]
        [Required(ErrorMessage = "Enter an outgoing trust SharePoint link")]
        [Display(Name = "Outgoing trust SharePoint link")]
        public string OutgoingTrustSharePointLink { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Enter a date for the Provisional Transfer Date, like 1 4 2023")]
        [Display(Name = "Provisional Transfer Date")]
        public DateTime? ProvisionalTransferDate { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "State if the conversion is due to 2RI. Choose yes or no")]
        [Display(Name = "IsDueTo2RI")]
        public bool? IsDueTo2RI { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "State if the transfer is due to an inadequate Ofsted rating. Choose yes or no")]
        [Display(Name = "Inadequate OfstedRating")]
        public bool? IsDueToInedaquateOfstedRating { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "State if the transfer is due to financial, safeguarding or governance issues. Choose yes or no")]
        [Display(Name = "Issues")]
        public bool? IsDueToIssues { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "State if the outgoing trust will close once this transfer is completed. Choose yes or no")]
        [Display(Name = "Will outgoing trust close")]
        public bool? OutgoingTrustWillClose { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "State if this project will be handed over to the Regional casework services team. Choose yes or no")]
        [Display(Name = "Is Handing To RCS")]
        public bool? IsHandingToRCS { get; set; }

        [BindProperty] 
        public string? HandoverComments { get; set; }



        public async Task<IActionResult> OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {

            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            }

            var userAdId = User.GetUserAdId();

            var createProjectCommand = new CreateTransferProjectCommand(
                Urn: new Urn(int.Parse(URN)),
                OutgoingTrustUkprn: new Ukprn(int.Parse(OutgoingUKPRN)),
                IncomingTrustUkprn: new Ukprn(int.Parse(IncomingUKPRN)),
                SignificantDate: ProvisionalTransferDate.HasValue ? DateOnly.FromDateTime(ProvisionalTransferDate.Value) : default,
                IsSignificantDateProvisional: true, // will be set to false in the stakeholder kick off task 
                IsDueTo2Ri: IsDueTo2RI ?? false,
                IsDueToInedaquateOfstedRating: IsDueToInedaquateOfstedRating ?? false,
                IsDueToIssues: IsDueToIssues ?? false,
                OutGoingTrustWillClose: OutgoingTrustWillClose ?? false,
                AdvisoryBoardDate: AdvisoryBoardDate.HasValue ? DateOnly.FromDateTime(AdvisoryBoardDate.Value) : default,
                AdvisoryBoardConditions: AdvisoryBoardConditions,
                EstablishmentSharepointLink: AcademySharePointLink,
                IncomingTrustSharepointLink: IncomingTrustSharePointLink,
                OutgoingTrustSharepointLink: OutgoingTrustSharePointLink,
                GroupReferenceNumber: GroupReferenceNumber,
                HandoverComments: HandoverComments,
                UserAdId: userAdId
            );

            var createResponse = await sender.Send(createProjectCommand);

            var projectId = createResponse.Value;

            return Redirect($"/transfer-projects/{projectId}/tasks");
        }
    }
}