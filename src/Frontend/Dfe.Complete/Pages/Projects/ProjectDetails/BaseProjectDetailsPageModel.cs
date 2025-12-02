using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Dfe.Complete.Pages.Projects.ProjectDetails
{
    public class BaseProjectDetailsPageModel(ISender sender, IErrorService errorService, ILogger _logger, IProjectPermissionService projectPermissionService) : BaseProjectPageModel(sender, _logger, projectPermissionService)
    {
        public IErrorService ErrorService
        {
            get
            {
                return errorService;
            }
        }

        [BindProperty] public string? EstablishmentName { get; set; } // Common

        [BindProperty] public ProjectType? ProjectType { get; set; } // Common

        [BindProperty]        
        [Ukprn]        
        [DisplayName("incoming trust UKPRN")]
        public string? IncomingTrustUkprn { get; set; }  // Common

        [BindProperty]
      
        public string? NewTrustReferenceNumber { get; set; }  // Common

        [BindProperty]        
        public string? OriginalTrustReferenceNumber { get; set; }  // Common

        [BindProperty]
        [GroupReferenceNumber(ShouldMatchWithTrustUkprn: true, nameof(IncomingTrustUkprn))]
        [Display(Name = "Group Reference Number")]
        public string? GroupReferenceNumber { get; set; } // Common

        [BindProperty]
        [Required(ErrorMessage = "Enter a date for the Advisory Board Date, like 1 4 2023")]
        [DateInPast]
        [Display(Name = "Date of advisory board")]
        public DateTime? AdvisoryBoardDate { get; set; } // Common

        [BindProperty]
        public string? AdvisoryBoardConditions { get; set; } // Common

        [BindProperty]
        [SharePointLink]
        [Required(ErrorMessage = "Enter a school SharePoint link")]
        [Display(Name = "School SharePoint folder link")]
        public string? EstablishmentSharepointLink { get; set; } // Common

        [BindProperty]
        [SharePointLink]
        [Required(ErrorMessage = "Enter an incoming trust SharePoint link")]
        [Display(Name = "Incoming trust SharePoint link")]
        public string? IncomingTrustSharepointLink { get; set; } // Common

        [BindProperty]
        [Required(ErrorMessage =
            "State if this project will be handed over to the Regional casework services team. Choose yes or no")]
        [Display(Name = "Is Handing To RCS")]
        public bool? IsHandingToRCS { get; set; } // Common

        [BindProperty]
        [Required(ErrorMessage = "State if the conversion is due to 2RI. Choose yes or no")]
        [Display(Name = "IsDueTo2RI")]
        public bool? TwoRequiresImprovement { get; set; }// Common

        protected async Task SetGroupReferenceNumberAsync()
        {
            if (Project.GroupId != null)
            {
                var projectGroupQuery = new GetProjectGroupByIdQuery(Project.GroupId);
                var projectGroup = await Sender.Send(projectGroupQuery);
                if (projectGroup != null && projectGroup.IsSuccess && projectGroup.Value != null)
                {
                    GroupReferenceNumber = projectGroup.Value.GroupIdentifier;
                }
            }
        }

        public override async Task<IActionResult> OnGetAsync()
        {
            var baseResult = await base.OnGetAsync();
            if (baseResult is not PageResult) return baseResult;

            EstablishmentName = Establishment?.Name;
            ProjectType = Project.Type;

            IncomingTrustUkprn = Project.IncomingTrustUkprn?.ToString();
            NewTrustReferenceNumber = Project.NewTrustReferenceNumber;
            OriginalTrustReferenceNumber = Project.NewTrustReferenceNumber;

            await SetGroupReferenceNumberAsync();

            AdvisoryBoardDate = Project.AdvisoryBoardDate?.ToDateTime(default);
            AdvisoryBoardConditions = Project.AdvisoryBoardConditions;
            EstablishmentSharepointLink = HttpUtility.UrlDecode(Project.EstablishmentSharepointLink);
            IncomingTrustSharepointLink = HttpUtility.UrlDecode(Project.IncomingTrustSharepointLink);
            IsHandingToRCS = Project.Team == ProjectTeam.RegionalCaseWorkerServices;
            TwoRequiresImprovement = Project.TwoRequiresImprovement ?? false;

            return Page();
        }

        public void ValidateTrustReferenceNumber()
        {
            if (!string.IsNullOrWhiteSpace(OriginalTrustReferenceNumber) && string.IsNullOrWhiteSpace(NewTrustReferenceNumber))
            {
                ModelState.AddModelError("NewTrustReferenceNumber", "Enter a trust reference number (TRN)");
            }
        }
    }
}
