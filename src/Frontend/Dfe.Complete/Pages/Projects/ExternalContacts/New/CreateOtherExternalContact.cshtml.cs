using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Application.LocalAuthorities.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Services.AcademiesApi;
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

public class CreateOtherExternalContact(ITrustCache trustCacheService, ErrorService errorService, ISender sender, ILogger<CreateOtherExternalContact> logger)
    : ExternalContactBasePageModel(sender, logger)
{
    [BindProperty]
    public OtherExternalContactInputModel ExternalContactInput { get; set; } = new();
    
    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();
        return this.GetPage();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return GetPage();
        }
        else
        {
            try
            {   
                
                var contactType = EnumExtensions.FromDescription<ExternalContactType>(this.ExternalContactInput.SelectedExternalContactType);               

                if ((contactType == ExternalContactType.Solicitor || contactType == ExternalContactType.Diocese || contactType == ExternalContactType.Other) && this.ExternalContactInput.IsPrimaryProjectContact)
                {
                    ModelState.AddModelError(nameof(this.ExternalContactInput.IsPrimaryProjectContact), "Only the incoming trust, outgoing trust and school or academy categories can have a primary contact.");
                    errorService.AddErrors(ModelState);
                    return GetPage();
                }

                var organisationName = string.Empty;                
                var category = ExternalContactHelper.GetCategoryByContactType(contactType);

                await base.GetCurrentProject();

                switch (contactType)
                {
                    case ExternalContactType.HeadTeacher:
                    case ExternalContactType.ChairOfGovernors:
                    case ExternalContactType.SchoolOrAcademy:
                        organisationName = this.Project?.EstablishmentName?.ToTitleCase();
                        break;
                    case ExternalContactType.IncomingTrustCEO:
                        if (!this.Project.FormAMat && Project.IncomingTrustUkprn != null)
                        {
                            var incomingTrust = await trustCacheService.GetTrustAsync(this.Project.IncomingTrustUkprn);
                            organisationName = incomingTrust.Name?.ToTitleCase();
                        }
                        break;
                    case ExternalContactType.OutgoingTrustCEO:
                        if (this.Project?.Type == ProjectType.Transfer && this.Project.OutgoingTrustUkprn != null)
                        {
                            var outgoingTrust = await trustCacheService.GetTrustAsync(this.Project.OutgoingTrustUkprn);
                            organisationName = outgoingTrust.Name?.ToTitleCase();
                        }
                        break;
                    case ExternalContactType.Solicitor:
                        organisationName = this.ExternalContactInput.OrganisationSolicitor;
                        break;
                    case ExternalContactType.Diocese:
                        organisationName = this.ExternalContactInput.OrganisationDiocese;
                        break;
                    case ExternalContactType.LocalAuthority:

                        var establishmentQuery = new GetEstablishmentByUrnRequest(this.Project.Urn.Value.ToString());
                        var establishmentResult = await Sender.Send(establishmentQuery);

                        if (establishmentResult.IsSuccess && establishmentResult.Value != null)
                        {
                            var laQuery = new GetLocalAuthorityByCodeQuery(establishmentResult.Value?.LocalAuthorityCode);

                            var la = await Sender.Send(laQuery);
                            if (la.IsSuccess && la.Value != null)
                            {
                                organisationName = la.Value?.Name;
                            }                           
                        }
                       
                        break;
                    default:
                        organisationName = this.ExternalContactInput.OrganisationOther;
                        break;
                }

                var newExternalContactCommand = new CreateExternalContactCommand(
                    FullName: this.ExternalContactInput.FullName,
                    Role: this.ExternalContactInput.Role,
                    Email: this.ExternalContactInput.Email ?? string.Empty,
                    PhoneNumber: this.ExternalContactInput.Phone ?? string.Empty,
                    Category: category,
                    IsPrimaryContact: this.ExternalContactInput.IsPrimaryProjectContact,
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

                return Redirect(string.Format(RouteConstants.ProjectExternalContacts, ProjectId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when creating a new external contact for project {ProjectId}", ProjectId);
                ModelState.AddModelError("UnexpectedError", "An unexpected error occurred. Please try again later.");
                return Page();
            }
        }       
    }


    private IActionResult GetPage()
    {
        this.SetExternalContactTypes();
        if(string.IsNullOrWhiteSpace(this.ExternalContactInput.SelectedExternalContactType))
        {
            this.ExternalContactInput.SelectedExternalContactType = ExternalContactType.Other.ToDescription();
        }
        return Page();
    }

    private void SetExternalContactTypes()
    {
        this.ExternalContactInput.ContactTypeRadioOptions = new[]
           {
                ExternalContactType.SchoolOrAcademy,
                ExternalContactType.IncomingTrustCEO,
                ExternalContactType.OutgoingTrustCEO,
                ExternalContactType.LocalAuthority,
                ExternalContactType.Solicitor,
                ExternalContactType.Diocese,
                ExternalContactType.Other,
            };
    }
}