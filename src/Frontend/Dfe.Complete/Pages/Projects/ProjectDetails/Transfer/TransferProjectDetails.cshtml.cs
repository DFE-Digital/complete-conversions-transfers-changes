using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Web;

namespace Dfe.Complete.Pages.Projects.ProjectDetails.Transfer
{
    public class TransferProjectDetailsModel(ISender sender, IErrorService errorService, ILogger<TransferProjectDetailsModel> _logger) : BaseProjectPageModel(sender, _logger)
    {
        public IErrorService ErrorService
        {
            get
            {
                return errorService;
            }
        }

        [BindProperty] public string? EstablishmentName { get; set; }  // Common

        [BindProperty]
        [GovukRequired]
        [Ukprn]
        [Required(ErrorMessage = "Enter an outgoing trust UKPRN")]
        [DisplayName("outgoing trust UKPRN")]
        public string? OutgoingTrustUkprn { get; set; }

        [BindProperty]
        [GovukRequired]
        [Ukprn]
        [Required(ErrorMessage = "Enter an incoming trust UKPRN")]
        [DisplayName("incoming trust UKPRN")]
        public string? IncomingTrustUkprn { get; set; }  // Common

        [BindProperty]
        public string? NewTrustReferenceNumber { get; set; }

        [BindProperty]
        [GroupReferenceNumber(ShouldMatchWithTrustUkprn: true, nameof(IncomingTrustUkprn))]
        [Display(Name = "Group Reference Number")]
        public string? GroupReferenceNumber { get; set; }  // Common

        [BindProperty]
        [Required(ErrorMessage = "Enter a date for the Advisory Board Date, like 1 4 2023")]
        [DateInPast]
        [Display(Name = "Date of advisory board")]
        public DateTime? AdvisoryBoardDate { get; set; }   // Common

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
        [SharePointLink]
        [Required(ErrorMessage = "Enter an outgoing trust SharePoint link")]
        [Display(Name = "Outgoing trust SharePoint link")]
        public string? OutgoingTrustSharepointLink { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "State if the conversion is due to 2RI. Choose yes or no")]
        [Display(Name = "IsDueTo2RI")]
        public bool? TwoRequiresImprovement { get; set; } // Common

        [BindProperty]
        [Required(ErrorMessage = "State if the transfer is due to an inadequate Ofsted rating. Choose yes or no")]
        [Display(Name = "Inadequate OfstedRating")]
        public bool? InadequateOfsted { get; set; }

        [BindProperty]
        [Required(ErrorMessage =
            "State if the transfer is due to financial, safeguarding or governance issues. Choose yes or no")]
        [Display(Name = "Issues")]
        public bool? FinancialSafeguardingGovernanceIssues { get; set; }

        [BindProperty]
        [Required(ErrorMessage =
            "State if the outgoing trust will close once this transfer is completed. Choose yes or no")]
        [Display(Name = "Will outgoing trust close")]
        public bool? OutgoingTrustWillClose { get; set; }

        [BindProperty]
        [Required(ErrorMessage =
            "State if this project will be handed over to the Regional casework services team. Choose yes or no")]
        [Display(Name = "Is Handing To RCS")]
        public bool? IsHandingToRCS { get; set; } // Common

        [BindProperty] public string? HandoverComments { get; set; } // Common

        private async Task SetGroupReferenceNumberAsync()// Common
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

        private async Task SetTransferTaskDataAsync()
        {
            if (Project.TasksDataId != null)
            {
                var transferTasksDataQuery = new GetTransferTasksDataByIdQuery(Project.TasksDataId);
                var transferTasksData = await Sender.Send(transferTasksDataQuery);
                if (transferTasksData.IsSuccess || transferTasksData.Value != null)
                {
                    var tasksData = transferTasksData.Value!;

                    OutgoingTrustWillClose = tasksData.OutgoingTrustToClose ?? false;
                    FinancialSafeguardingGovernanceIssues = tasksData.FinancialSafeguardingGovernanceIssues ?? false;
                    InadequateOfsted = tasksData.InadequateOfsted ?? false;
                }
            }
        }

        public override async Task<IActionResult> OnGetAsync()
        {
            var baseResult = await base.OnGetAsync();
            if (baseResult is not PageResult) return baseResult;

            EstablishmentName = Establishment?.Name;// Common

            OutgoingTrustUkprn = Project.OutgoingTrustUkprn?.ToString();
            IncomingTrustUkprn = Project.IncomingTrustUkprn?.ToString();// Common
            NewTrustReferenceNumber = Project.NewTrustReferenceNumber;// Common

            await SetGroupReferenceNumberAsync();

            AdvisoryBoardDate = Project.AdvisoryBoardDate?.ToDateTime(default);// Common
            AdvisoryBoardConditions = Project.AdvisoryBoardConditions;// Common
            EstablishmentSharepointLink = HttpUtility.UrlDecode(Project.EstablishmentSharepointLink);// Common
            IncomingTrustSharepointLink = HttpUtility.UrlDecode(Project.IncomingTrustSharepointLink);// Common
            OutgoingTrustSharepointLink = HttpUtility.UrlDecode(Project.OutgoingTrustSharepointLink);

            TwoRequiresImprovement = Project.TwoRequiresImprovement ?? false; // Common

            await SetTransferTaskDataAsync();

            IsHandingToRCS = Project.Team == Domain.Enums.ProjectTeam.RegionalCaseWorkerServices;// Common
            HandoverComments = Project.Notes.FirstOrDefault()?.Body;// Common

            return Page();
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        errorService.AddErrors(ModelState);
        //        return Page();
        //    }

        //    return Page();
        //}
    }
}
