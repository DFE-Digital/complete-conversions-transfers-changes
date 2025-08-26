using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Helpers;
using Dfe.Complete.Models;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ExternalContacts.New;

public class CreateExternalContact(ISender sender, ITrustCache TrustCacheService,  ErrorService errorService, ILogger<CreateExternalContact> logger)
    : ExternalContactBasePageModel(sender, logger)
{   
    protected readonly ITrustCache trustCacheService = TrustCacheService;

    [FromQuery(Name = "externalcontacttype")]
    public string? SelectedExternalContactType { get; set; }

    [BindProperty]
    public ExternalContactDetailsModel ExternalContactDetails { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {   
        await base.OnGetAsync();

        if (string.IsNullOrWhiteSpace(SelectedExternalContactType))
        {
            this.SelectedExternalContactType = ExternalContactType.Other.ToDescription();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return Page();
        }
        else
        {
            try
            {
                await base.GetCurrentProject();

                var contactType = EnumExtensions.FromDescription<ExternalContactType>(this.SelectedExternalContactType);
                var organisationName = string.Empty;
                var role = ExternalContactHelper.GetRoleByContactType(contactType);
                var category = ExternalContactHelper.GetCategoryByContactType(contactType);

                switch (contactType)
                {
                    case ExternalContactType.HeadTeacher:
                    case ExternalContactType.ChairOfGovernors:
                        organisationName = this.Project?.EstablishmentName?.ToTitleCase();
                        break;
                    case ExternalContactType.IncomingTrustCEO:
                        if (!this.Project.FormAMat && Project.IncomingTrustUkprn != null)
                        {  
                            var incomingTrust = await this.trustCacheService.GetTrustAsync(this.Project.IncomingTrustUkprn);
                            organisationName = incomingTrust.Name?.ToTitleCase();
                        }
                        break;
                    case ExternalContactType.OutgoingTrustCEO:
                        if (this.Project?.Type == ProjectType.Transfer && this.Project.OutgoingTrustUkprn != null)
                        {
                            var outgoingTrust = await this.trustCacheService.GetTrustAsync(this.Project.OutgoingTrustUkprn);
                            organisationName = outgoingTrust.Name?.ToTitleCase();
                        }
                        break;                    
                    default:
                        ModelState.AddModelError("InvalidContactType", "The selected contact type is not valid.");
                        return Page();
                }

                var newExternalContactCommand = new CreateExternalContactCommand(
                    FullName: this.ExternalContactDetails.FullName,
                    Role: role,
                    Email: this.ExternalContactDetails.Email ?? string.Empty,
                    PhoneNumber: this.ExternalContactDetails.Phone ?? string.Empty,
                    Category: category,
                    IsPrimaryContact: this.ExternalContactDetails.IsPrimaryProjectContact,
                    ProjectId: new ProjectId(Guid.Parse(this.ProjectId)),
                    EstablishmentUrn: null,
                    OrganisationName: organisationName,
                    LocalAuthorityId: null,
                    Type: ContactType.Project
                );

                var response = await Sender.Send(newExternalContactCommand);
                var contactId = response.Value;

                TempData.SetNotification(
                    NotificationType.Success,
                    "Success",
                    "Contact added"
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when creating a new external contact for project {ProjectId}", ProjectId);
                ModelState.AddModelError("UnexpectedError", "An unexpected error occurred. Please try again later.");
                return Page();
            }

            return Redirect(string.Format(RouteConstants.ProjectExternalContacts, ProjectId));
        }
    }
}