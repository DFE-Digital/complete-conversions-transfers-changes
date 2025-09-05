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

public class EditExternalContact(ITrustCache trustCacheService, ErrorService errorService, ISender sender, ILogger<EditExternalContact> logger) : ExternalContactAddEditPageModel(trustCacheService, sender, logger)
{
    private readonly ErrorService errorService = errorService;
    private readonly ISender sender = sender;
    private readonly ILogger<EditExternalContact> logger = logger;

    [BindProperty(SupportsGet = true, Name = "contactId")]
    public string ContactId { get; set; }

    public async override Task<IActionResult> OnGetAsync()
    {        
        return await GetPage();
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

                await base.GetCurrentProject();

                var organisationName = await this.GetOrganisationName(contactType);
                var category = ExternalContactMapper.MapContactTypeToCategory(contactType);

                var contactDto = new ContactDto
                {
                    Name = this.ExternalContactInput.FullName,
                    Title = this.ExternalContactInput.Role,
                    Email = this.ExternalContactInput.Email ?? string.Empty,
                    Phone = this.ExternalContactInput.Phone ?? string.Empty,
                    Category = category,
                    PrimaryContact = this.ExternalContactInput.IsPrimaryProjectContact,
                    OrganisationName = organisationName
                };

                var updateExternalContactCommand = new UpdateExternalContactCommand(new ContactId (Guid.Parse(ContactId)), contactDto );

                var response = await sender.Send(updateExternalContactCommand);
                
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
        this.ExternalContactInput.FullName = contactDto.Name;
        this.ExternalContactInput.Role = contactDto.Title;
        this.ExternalContactInput.Email = contactDto.Email;
        this.ExternalContactInput.Phone = contactDto.Phone;        

        var externalContactType = ExternalContactMapper.MapCategoryToContactType(contactDto.Category);
        this.ExternalContactInput.SelectedExternalContactType = externalContactType.ToDescription();

        this.ExternalContactInput.IsPrimaryProjectContact = IsExternalContactPrimaryContact(externalContactType, contactDto);

        switch (externalContactType)
        {
            case ExternalContactType.Solicitor:
                this.ExternalContactInput.OrganisationSolicitor = contactDto.OrganisationName;
                break;
            case ExternalContactType.Diocese:     
              this.ExternalContactInput.OrganisationDiocese = contactDto.OrganisationName;
              break;
            case ExternalContactType.Other:
                this.ExternalContactInput.OrganisationOther = contactDto.OrganisationName;
                break;
            default:                      
                break;
        }
    }

    private bool IsExternalContactPrimaryContact(ExternalContactType externalContactType, ContactDto contactDto)
    {
        return externalContactType switch
        {
            ExternalContactType.SchoolOrAcademy => this.Project?.EstablishmentMainContactId == contactDto.Id,
            ExternalContactType.IncomingTrust => this.Project?.IncomingTrustMainContactId == contactDto.Id,
            ExternalContactType.OutgoingTrust => this.Project?.OutgoingTrustMainContactId == contactDto.Id,
            ExternalContactType.LocalAuthority => this.Project?.LocalAuthorityMainContactId == contactDto.Id,
            _ => false
        };
    }
}