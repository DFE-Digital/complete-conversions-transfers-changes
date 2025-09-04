using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
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

namespace Dfe.Complete.Pages.Projects.ExternalContacts;

public class EditExternalContact(ITrustCache trustCacheService, ErrorService errorService, ISender sender, ILogger<EditExternalContact> logger)
     : ExternalContactAddEditPageModel(trustCacheService, sender, logger)
{
    [BindProperty(SupportsGet = true, Name = "contactId")]
    public string ContactId { get; set; }

    public async override Task<IActionResult> OnGetAsync()
    {        
        return await this.GetPage();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return await this.OnGetAsync();
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

                var organisationName = string.Empty;
                var category = ExternalContactMapper.MapContactTypeToCategory(contactType);

                await base.GetCurrentProject();

                //var editExternalContactCommand = new CreateExternalContactCommand(
                //    FullName: this.ExternalContactInput.FullName,
                //    Role: this.ExternalContactInput.Role,
                //    Email: this.ExternalContactInput.Email ?? string.Empty,
                //    PhoneNumber: this.ExternalContactInput.Phone ?? string.Empty,
                //    Category: category,
                //    IsPrimaryContact: this.ExternalContactInput.IsPrimaryProjectContact,
                //    ProjectId: new ProjectId(Guid.Parse(this.ProjectId)),
                //    EstablishmentUrn: null,
                //    OrganisationName: organisationName,
                //    LocalAuthorityId: null,
                //    Type: ContactType.Project
                //);

                //var response = await sender.Send(editExternalContactCommand);
                //var contactId = response.Value;

                TempData.SetNotification(
                    NotificationType.Success,
                    "Success",
                    "Contact updated"
                );

                return Redirect(string.Format(RouteConstants.ProjectExternalContacts, ProjectId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when creating a new external contact for project {ProjectId}", ProjectId);
                ModelState.AddModelError("UnexpectedError", "An unexpected error occurred. Please try again later.");
                return await this.GetPage();
            }
        }
    }      

    protected async Task<IActionResult> GetPage()
    {
        await base.OnGetAsync();
        await this.GetContactDetails();
        return Page();
    }

    private async Task GetContactDetails()
    {
        var success = Guid.TryParse(ContactId, out var guid);

        if (!success)
        {
            var error = $"{ContactId} is not a valid Guid.";
            logger.LogError(error);
            return;
        }

        var query = new GetContactByIdQuery(new ContactId(guid));
        var result = await sender.Send(query);

        if (!result.IsSuccess || result.Value == null)
        {
            var error = $"Contact {ContactId} does not exist.";
            logger.LogError(error);
            return;
        }

        MapContactDtoToModel(result.Value);
    }

    private void MapContactDtoToModel(ContactDto contactDto)
    {
        var model = new OtherExternalContactInputModel
        {
            FullName = contactDto.Name,
            Role = contactDto.Title,
            Email = contactDto.Email,
            Phone = contactDto.Phone
        };

        var externalContactType = ExternalContactMapper.MapCategoryToContactType(contactDto.Category);
        model.SelectedExternalContactType = externalContactType.ToDescription();
        
        this.ExternalContactInput.IsPrimaryProjectContact = IsExternalContactPrimaryContact(externalContactType, contactDto);
    }

    private bool IsExternalContactPrimaryContact(ExternalContactType externalContactType, ContactDto contactDto)
    {
        return externalContactType switch
        {
            ExternalContactType.SchoolOrAcademy => this.Project?.EstablishmentMainContactId == contactDto.Id,
            ExternalContactType.IncomingTrust => this.Project?.IncomingTrustMainContactId == contactDto.Id,
            ExternalContactType.OutgoingTrust => this.Project?.OutgoingTrustMainContactId == contactDto.Id,
            _ => false
        };
    }
}