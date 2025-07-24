using Azure.Core;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Notes.Queries;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Dfe.Complete.Pages.Projects.ProjectDetails.Conversion
{

    public class ConversationProjectDetailsModel(ISender sender, IErrorService errorService, ILogger<ConversationProjectDetailsModel> _logger) : BaseProjectPageModel(sender, _logger)
    {
        public IErrorService ErrorService
        {
            get
            {
                return errorService;
            }
        }

        [BindProperty] public string? EstablishmentName { get; set; } // Common

        [BindProperty]
        [GovukRequired]
        [Ukprn]
        [Required(ErrorMessage = "Enter an incoming trust UKPRN")]
        [DisplayName("incoming trust UKPRN")]
        public string? IncomingTrustUkprn { get; set; }  // Common

        [BindProperty]
        public string? NewTrustReferenceNumber { get; set; }  // Common

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

        [BindProperty] public string? HandoverComments { get; set; } // Common

        [BindProperty]
        [Required(ErrorMessage =
            "Select directive academy order or academy order, whichever has been used for this conversion")]
        [Display(Name = "Directive Academy Order")]
        public bool? DirectiveAcademyOrder { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "State if the conversion is due to 2RI. Choose yes or no")]
        [Display(Name = "IsDueTo2RI")]
        public bool? TwoRequiresImprovement { get; set; }// Common

        private async Task SetGroupReferenceNumberAsync()
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

        private async Task SetHandoverComments()
        {
            var projectId = new ProjectId(Guid.Parse(ProjectId));
            var query = new GetHandoverNotesByProjectIdQuery(projectId);
            var notesResult = await Sender.Send(query);

            if (notesResult.IsSuccess)
            {
                HandoverComments = (notesResult.Value ?? [])
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault()?
                    .Body;
            }
        }

        public override async Task<IActionResult> OnGetAsync()
        {
            var baseResult = await base.OnGetAsync();
            if (baseResult is not PageResult) return baseResult;

            EstablishmentName = Establishment?.Name;

            IncomingTrustUkprn = Project.IncomingTrustUkprn?.ToString();
            NewTrustReferenceNumber = Project.NewTrustReferenceNumber;

            await SetGroupReferenceNumberAsync();

            AdvisoryBoardDate = Project.AdvisoryBoardDate?.ToDateTime(default);
            AdvisoryBoardConditions = Project.AdvisoryBoardConditions;
            EstablishmentSharepointLink = HttpUtility.UrlDecode(Project.EstablishmentSharepointLink);
            IncomingTrustSharepointLink = HttpUtility.UrlDecode(Project.IncomingTrustSharepointLink);
            IsHandingToRCS = Project.Team == Domain.Enums.ProjectTeam.RegionalCaseWorkerServices;

            await SetHandoverComments();

            DirectiveAcademyOrder = Project.DirectiveAcademyOrder ?? false;
            TwoRequiresImprovement = Project.TwoRequiresImprovement ?? false;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            }

            var user = await sender.Send(new GetUserByAdIdQuery(User.GetUserAdId()), cancellationToken);

            if (user is not { IsSuccess: true })
                throw new NotFoundException("No user found.", innerException: new Exception(user?.Error));


            var projectId = new ProjectId(Guid.Parse(ProjectId));

            var updateProjectCommand = new UpdateConversionProjectCommand(
                ProjectId: projectId,
                IncomingTrustUkprn: new Ukprn(IncomingTrustUkprn!.ToInt()),
                NewTrustReferenceNumber: NewTrustReferenceNumber,
                GroupIdentifier: !string.IsNullOrEmpty(GroupReferenceNumber) ? new ProjectGroupId(Guid.Parse(GroupReferenceNumber)) : null,
                AdvisoryBoardDate: AdvisoryBoardDate.HasValue
                    ? DateOnly.FromDateTime(AdvisoryBoardDate.Value)
                    : default,
                AdvisoryBoardConditions: AdvisoryBoardConditions ?? string.Empty,
                EstablishmentSharepointLink: EstablishmentSharepointLink ?? string.Empty,
                IncomingTrustSharepointLink: IncomingTrustSharepointLink ?? string.Empty,
                IsHandingToRCS: IsHandingToRCS ?? false,
                HandoverComments: HandoverComments ?? string.Empty,
                DirectiveAcademyOrder: DirectiveAcademyOrder ?? false,
                TwoRequiresImprovement: TwoRequiresImprovement ?? false,
                User: user.Value!
            );

            await Sender.Send(updateProjectCommand, cancellationToken);

            TempData.SetNotification(
                NotificationType.Success,
                "Success",
                "Project has been updated successfully");

            return Redirect(string.Format(RouteConstants.ProjectAbout, ProjectId));
        }
    }
}
