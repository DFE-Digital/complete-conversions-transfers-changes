using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.MatConversion;

[Authorize(policy: "CanCreateProjects")]
public class CreateNewProject(ISender sender, ErrorService errorService, ILogger<CreateNewProject> logger) : PageModel
{
    [BindProperty]
    [Urn]
    [Required(ErrorMessage = "Enter a school URN")]
    [Display(Name = "Urn")]
    public string URN { get; set; }

    [BindProperty]
    [Trn]
    [Required(ErrorMessage = "Enter a Trust reference number (TRN)")]
    [Display(Name = "Trust reference number (TRN)")]
    public string TrustReferenceNumber { get; set; }

    [BindProperty]
    [Display(Name = "Trust name")]
    [Required(ErrorMessage = "Enter a Trust name")]
    public string TrustName { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Enter a date for the Advisory Board Date, like 1 4 2023")]
    [DateInPast]
    [Display(Name = "Advisory Board Date")]
    public DateTime? AdvisoryBoardDate { get; set; }

    [BindProperty] public string? AdvisoryBoardConditions { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Enter a date for the Provisional Conversion Date, like 1 4 2023")]
    [Display(Name = "Provisional Conversion Date")]
    public DateTime? SignificantDate { get; set; }


    [BindProperty]
    [SharePointLink]
    [Required(ErrorMessage = "Enter a school sharepoint link")]
    [Display(Name = "School or academy SharePoint link")]
    public string SchoolSharePointLink { get; set; }

    [BindProperty]
    [SharePointLink]
    [Required(ErrorMessage = "Enter an incoming trust Sharepoint link")]
    [Display(Name = "Incoming trust SharePoint link")]
    public string IncomingTrustSharePointLink { get; set; }

    [BindProperty]
    [Required(ErrorMessage =
        "State if this project will be handed over to the Regional casework services team. Choose yes or no")]
    [Display(Name = "Is Handing To RCS")]
    public bool? IsHandingToRCS { get; set; }

    [BindProperty] public string? HandoverComments { get; set; }

    [BindProperty]
    [Required(ErrorMessage =
        "Select directive academy order or academy order, whichever has been used for this conversion")]
    [Display(Name = "Directive Academy Order")]
    public bool? DirectiveAcademyOrder { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "State if the conversion is due to 2RI. Choose yes or no")]
    [Display(Name = "IsDueTo2RI")]
    public bool? IsDueTo2RI { get; set; }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return Page();
        }

        try
        {
            var userAdId = User.GetUserAdId();

            var createProjectCommand = new CreateMatConversionProjectCommand(
                Urn: new Urn(int.Parse(URN)),
                TrustName,
                TrustReferenceNumber,
                SignificantDate: SignificantDate.HasValue
                    ? DateOnly.FromDateTime(SignificantDate.Value)
                    : default,
                IsSignificantDateProvisional: true, // will be set to false in the stakeholder kick off task 
                IncomingTrustSharepointLink: IncomingTrustSharePointLink,
                EstablishmentSharepointLink: SchoolSharePointLink,
                IsDueTo2Ri: IsDueTo2RI ?? false,
                AdvisoryBoardDate: AdvisoryBoardDate.HasValue
                    ? DateOnly.FromDateTime(AdvisoryBoardDate.Value)
                    : default,
                AdvisoryBoardConditions: AdvisoryBoardConditions ?? string.Empty,
                HasAcademyOrderBeenIssued: DirectiveAcademyOrder ?? default,
                HandingOverToRegionalCaseworkService: IsHandingToRCS ?? default,
                HandoverComments: HandoverComments,
                UserAdId: userAdId
            );

            var createResponse = await sender.Send(createProjectCommand, cancellationToken);

            var projectId = createResponse.Value;

            return Redirect($"/projects/conversion-projects/{projectId}/created");
        }
        catch (NotFoundException notFoundException)
        {
            logger.LogError(notFoundException, notFoundException.Message, notFoundException.InnerException);
                
            ModelState.AddModelError(notFoundException.Field ?? "NotFound", notFoundException.Message);

            errorService.AddErrors(ModelState);
            
            return Page();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating a conversion project.");
            ModelState.AddModelError("UnexpectedError", "An unexpected error occurred. Please try again later.");
            return Page();
        }
    }
}