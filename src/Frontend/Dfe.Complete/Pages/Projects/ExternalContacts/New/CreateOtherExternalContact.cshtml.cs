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

public class CreateOtherExternalContact(ITrustCache trustCacheService, ErrorService errorService, ISender sender, ILogger<CreateOtherExternalContact> logger)
    : ExternalContactAddEditPageModel(trustCacheService, sender, logger)
{
    private readonly ISender _sender = sender;    

    public async override Task<IActionResult> OnGetAsync()
    {   
        return await this.GetPage();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return await this.GetPage();
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
                    return await this.GetPage();
                }

                await this.GetCurrentProject();

                var category = ExternalContactMapper.MapContactTypeToCategory(contactType);
                var organisationName = await this.GetOrganisationName(contactType);

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

                var response = await _sender.Send(newExternalContactCommand);
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
                return await GetPage();
            }
        }
    }

    private async Task<IActionResult> GetPage()
    {
        await base.OnGetAsync();
        return Page();
    }
}
