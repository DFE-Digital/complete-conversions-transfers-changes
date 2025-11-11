using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Helpers;
using Dfe.Complete.Models;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using Dfe.Complete.Utils.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValidationConstants = Dfe.Complete.Constants.ValidationConstants;

namespace Dfe.Complete.Pages.Projects.ExternalContacts;

[Authorize(Policy = UserPolicyConstants.CanViewEditDeleteContact)]
public class EditExternalContact(
    IValidator<OtherExternalContactInputModel> otherExternalContactInputModelValidator,
    ITrustCache trustCacheService, IErrorService errorService,
    ISender sender,
    ILogger<EditExternalContact> logger) : ExternalContactAddEditPageModel(trustCacheService, sender)
{
    private readonly IErrorService errorService = errorService;
    private readonly ISender sender = sender;
    private readonly ILogger<EditExternalContact> logger = logger;
    private readonly IValidator<OtherExternalContactInputModel> validator = otherExternalContactInputModelValidator;

    [BindProperty(SupportsGet = true, Name = "contactId")]
    public string ContactId { get; set; }

    public async override Task<IActionResult> OnGetAsync()
    {
        return await GetPage();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await validator.ValidateAsync(ExternalContactInput);

        if (!result.IsValid)
            return await HandleValidationFailure(result);

        try
        {
            var updateCommand = await BuildUpdateCommandAsync();
            var response = await sender.Send(updateCommand);

            TempData.SetNotification(
                NotificationType.Success,
                "Success",
                "Contact updated"
            );

            return Redirect(string.Format(RouteConstants.ProjectExternalContacts, ProjectId));
        }
        catch (Exception ex)
        {
            return await HandleUnexpectedErrorAsync(ex);
        }
    }

    protected async Task<IActionResult> GetPage()
    {
        await base.OnGetAsync();
        await this.GetContactDetails();
        return Page();
    }

    private async Task<IActionResult> HandleValidationFailure(FluentValidation.Results.ValidationResult result)
    {
        AddValidationErrorsToModelState(result);
        errorService.AddErrors(ModelState);
        await base.OnGetAsync();

        return Page();
    }

    private async Task<UpdateExternalContactCommand> BuildUpdateCommandAsync()
    {
        var contactType = EnumExtensions.FromDescription<ExternalContactType>(ExternalContactInput.SelectedExternalContactType);

        await base.GetCurrentProjectAsync();

        var organisationName = await GetOrganisationNameAsync(contactType);
        var category = ExternalContactMapper.MapContactTypeToCategory(contactType);

        var contactDto = new ContactDto
        {
            Name = ExternalContactInput.FullName,
            Title = ExternalContactInput.Role,
            Email = ExternalContactInput.Email ?? string.Empty,
            Phone = ExternalContactInput.Phone ?? string.Empty,
            Category = category,
            PrimaryContact = ExternalContactInput.IsPrimaryProjectContact,
            OrganisationName = organisationName
        };

        return new UpdateExternalContactCommand(
            new ContactId(Guid.Parse(ContactId)),
            contactDto
        );
    }

    private async Task<IActionResult> HandleUnexpectedErrorAsync(Exception ex)
    {
        logger.LogError(ex, "An error occurred while creating a new external contact for project {ProjectId}", ProjectId);
        ModelState.AddModelError("UnexpectedError", "An unexpected error occurred. Please try again later.");
        errorService.AddErrors(ModelState);
        await base.OnGetAsync();

        return Page();
    }

    private async Task GetContactDetails()
    {
        var success = Guid.TryParse(ContactId, out var guid);

        if (!success)
        {
            var error = string.Format(ValidationConstants.InvalidGuid, ContactId);
            throw new NotFoundException(error);
        }

        var query = new GetContactByIdQuery(new ContactId(guid));
        var result = await sender.Send(query);

        if (!result.IsSuccess || result.Value == null)
        {
            var error = string.Format(ValidationConstants.ContactNotFound, ContactId);
            throw new NotFoundException(error);
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