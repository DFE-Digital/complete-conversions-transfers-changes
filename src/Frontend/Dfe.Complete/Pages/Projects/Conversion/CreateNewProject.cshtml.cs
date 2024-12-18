using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;
using Dfe.Complete.Extensions;
using Dfe.Complete.Validators;
using Dfe.Complete.Services;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using MediatR;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Pages.Projects.Conversion
{
    public class CreateNewProjectModel(ISender sender, IErrorService errorService) : PageModel
    {
        [BindProperty] public string URN { get; set; }

        [BindProperty] public string UKPRN { get; set; }

        [BindProperty] public string GroupReferenceNumber { get; set; }

        [BindProperty] public DateTime? AdvisoryBoardDate { get; set; }

        [BindProperty] public string AdvisoryBoardConditions { get; set; }

        [BindProperty] public DateTime? ProvisionalConversionDate { get; set; }

        [BindProperty]
        [SharePointLink]
        [Required]
        [Display(Name = "School or academy SharePoint link")]
        public string SchoolSharePointLink { get; set; }

        [BindProperty]
        [SharePointLink]
        [Required]
        [Display(Name = "Incoming trust SharePoint link")]
        public string IncomingTrustSharePointLink { get; set; }

        [BindProperty] public bool? IsHandingToRCS { get; set; }

        [BindProperty] public string HandoverComments { get; set; }

        [BindProperty] public bool? DirectiveAcademyOrder { get; set; }

        [BindProperty] public bool? IsDueTo2RI { get; set; }

        public async Task<IActionResult> OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            //Validate
            await ValidateAllFields();

            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            }

            var createProjectCommand = new CreateConversionProjectCommand(
                Urn: new Urn(2),
                SignificantDate: DateOnly.FromDateTime(DateTime.UtcNow),
                IsSignificantDateProvisional: true, // will be set to false in the stakeholder kick off task 
                IncomingTrustSharepointLink: "https://www.sharepointlink.com/test",
                EstablishmentSharepointLink: "https://www.sharepointlink.com/test",
                IsDueTo2Ri: false,
                AdvisoryBoardDate: DateOnly.FromDateTime(DateTime.UtcNow),
                Region: Domain.Enums.Region.NorthWest,
                AdvisoryBoardConditions: "test conditions",
                IncomingTrustUkprn: new Ukprn(2),
                HasAcademyOrderBeenIssued: true
            );

            var createResponse = await sender.Send(createProjectCommand);

            var projectId = createResponse.Value;

            return Redirect($"/projects/conversion-projects/{projectId}/created");
        }

        public async Task ValidateAllFields()
        {
            await ValidateUrn();
            ValidateUKPRN();
            ValidateGroupReferenceNumber();
            ValidateAdvisoryBoardDate();
            ValidateProvisionalConversionDate();
            //TODO:EA needs fixing
            //ValidateSharePointLink(nameof(SchoolSharePointLink));
            //ValidateSharePointLink(nameof(IncomingTrustSharePointLink));
            ValidateHandingToRCS();
            ValidateAcademyOrder();
            ValidateDueTo2RI();
        }

        private void ValidateGroupReferenceNumber()
        {
            const string fieldName = nameof(GroupReferenceNumber);
            var value = GroupReferenceNumber;

            const string errorMessage = "A group group reference number must start GRP_ and contain 8 numbers, like GRP_00000001";

            if (!value.StartsWith("GRP_"))
            {
                ModelState.AddModelError($"{fieldName}", errorMessage);
                return;
            }

            var numberPortionOfRefNumber = value.Split("GRP_")[1];

            if (!int.TryParse(numberPortionOfRefNumber, NumberStyles.None, CultureInfo.InvariantCulture, out _) 
                || numberPortionOfRefNumber.Length < 8)
                ModelState.AddModelError($"{fieldName}", errorMessage);
        }

        private async Task ValidateUrn()
        {
            var fieldName = nameof(URN);
            var value = URN;

            if (string.IsNullOrEmpty(value))
            {
                ModelState.AddModelError($"{fieldName}", "Enter a school URN");
                return;
            }

            string pattern = "^[0-9]{6}$";

            var isAMatch = Regex.IsMatch(value, pattern);

            if (!isAMatch)
            {
                ModelState.AddModelError($"{fieldName}", "must be 6 digits");
                return;
            }

            var parsedUrn = value.ToInt();

            // var existingProject = await _getConversionProjectService.GetConversionProjectByUrn(parsedUrn);
            //
            // if (existingProject != null)
            // {
            //     ModelState.AddModelError($"{fieldName}", "project with this URN already exists");
            // }
        }

        private void ValidateUKPRN()
        {
            const string fieldName = nameof(UKPRN);
            var value = UKPRN;

            if (string.IsNullOrEmpty(value))
                ModelState.AddModelError($"{fieldName}", "Enter a UKPRN");
        }

        private void ValidateAdvisoryBoardDate()
        {
            var fieldName = nameof(AdvisoryBoardDate);
            var value = AdvisoryBoardDate;

            if (value == null || value == DateTime.MinValue)
            {
                ModelState.AddModelError($"{fieldName}", "Enter a date for the advisory board, like 1 4 2023");
                return;
            }
        }

        private void ValidateProvisionalConversionDate()
        {
            var fieldName = nameof(ProvisionalConversionDate);
            var value = ProvisionalConversionDate;

            if (value == null || value == DateTime.MinValue)
            {
                ModelState.AddModelError($"{fieldName}",
                    "Enter a month and year for the provisional conversion date, like 9 2023");
                return;
            }
        }

        private void ValidateSharePointLink(string fieldName)
        {
            var value = fieldName == nameof(SchoolSharePointLink) ? SchoolSharePointLink : IncomingTrustSharePointLink;

            if (string.IsNullOrEmpty(value))
            {
                ModelState.AddModelError($"{fieldName}", $"Enter a {fieldName} SharePoint link");
                return;
            }

            if (!value.Contains("https"))
            {
                ModelState.AddModelError($"{fieldName}", $"The SharePoint link must have the https scheme");
                return;
            }

            if (!value.Contains("https://educationgovuk.sharepoint.com") ||
                !value.Contains("https://educationgovuk-my.sharepoint.com/"))
            {
                ModelState.AddModelError($"{fieldName}",
                    $"Enter an incoming trust sharepoint link in the correct format. SharePoint links start with 'https://educationgovuk.sharepoint.com' or 'https://educationgovuk-my.sharepoint.com/'");
                return;
            }
        }

        private void ValidateHandingToRCS()
        {
            var fieldName = nameof(IsHandingToRCS);
            var value = IsHandingToRCS;

            if (value == null)
            {
                ModelState.AddModelError($"{fieldName}",
                    "State if this project will be handed over to the Regional casework services team. Choose yes or no");
                return;
            }
        }

        private void ValidateAcademyOrder()
        {
            var fieldName = nameof(DirectiveAcademyOrder);
            var value = DirectiveAcademyOrder;

            if (value == null)
            {
                ModelState.AddModelError($"{fieldName}",
                    "Select directive academy order or academy order, whichever has been used for this conversion");
                return;
            }
        }

        private void ValidateDueTo2RI()
        {
            var fieldName = nameof(IsDueTo2RI);
            var value = IsDueTo2RI;

            if (value == null)
            {
                ModelState.AddModelError($"{fieldName}", "State if the conversion is due to 2RI. Choose yes or no");
                return;
            }
        }
    }
}