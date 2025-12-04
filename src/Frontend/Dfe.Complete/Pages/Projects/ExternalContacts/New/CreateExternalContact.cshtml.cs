using Dfe.Complete.Application.Contacts.Commands;
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
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ExternalContacts.New;

[Authorize(Policy = UserPolicyConstants.CanAddContact)]
public class CreateExternalContact(
    IValidator<ExternalContactInputModel> externalContactInputModelValidator,
    ITrustCache trustCacheService,
    IErrorService errorService,
    ISender sender,
    ILogger<CreateExternalContact> logger)
    : ExternalContactBasePageModel(sender, trustCacheService)
{
    private const string invalidContactTypeErrorMessage = "The selected contact type is invalid";
    private readonly IErrorService errorService = errorService;
    private readonly ISender sender = sender;
    private readonly ILogger<CreateExternalContact> logger = logger;
    private readonly IValidator<ExternalContactInputModel> validator = externalContactInputModelValidator;

    [BindProperty]
    public ExternalContactInputModel ExternalContactInput { get; set; } = new();

    [FromRoute(Name = "externalcontacttype")]
    [BindProperty(SupportsGet = true, Name = "externalcontacttype")]
    public string SelectedExternalContactType { get; set; }

    public async virtual Task<IActionResult> OnGetAsync()
    {
        return await this.GetPageAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(ExternalContactInput);

        if (!validationResult.IsValid)
        {
            AddValidationErrorsToModelState(validationResult);
            errorService.AddErrors(ModelState);
            return await GetPageAsync();
        }

        try
        {
            await base.GetCurrentProjectAsync();

            var contactType = EnumExtensions.FromDescription<ExternalContactType>(SelectedExternalContactType);

            if (contactType == default)
            {
                ModelState.AddModelError("InvalidContactType", invalidContactTypeErrorMessage);
                errorService.AddErrors(ModelState);
                return await GetPageAsync();
            }

            var organisationName = await GetOrganisationNameAsync(contactType);

            if (organisationName == null && RequiresOrganisationName(contactType))
            {
                ModelState.AddModelError("InvalidContactType", invalidContactTypeErrorMessage);
                errorService.AddErrors(ModelState);
                return await GetPageAsync();
            }

            var command = BuildCreateContactCommand(contactType, organisationName!);

            var response = await sender.Send(command);
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
            return await GetPageAsync();
        }
    }

    private bool RequiresOrganisationName(ExternalContactType contactType)
        => contactType is ExternalContactType.IncomingTrust or ExternalContactType.OutgoingTrust;

    private async Task<string?> GetOrganisationNameAsync(ExternalContactType contactType)
    {
        return contactType switch
        {
            ExternalContactType.HeadTeacher or ExternalContactType.ChairOfGovernors
                => Project?.EstablishmentName?.ToTitleCase(),

            ExternalContactType.IncomingTrust 
                => await GetIncomingTrustNameAsync(),

            ExternalContactType.OutgoingTrust when Project?.Type == ProjectType.Transfer && Project.OutgoingTrustUkprn != null
                => (await trustCacheService.GetTrustAsync(Project.OutgoingTrustUkprn))?.Name?.ToTitleCase(),

            _ => null
        };
    }

    private CreateExternalContactCommand BuildCreateContactCommand(ExternalContactType contactType, string organisationName)
    {
        return new CreateExternalContactCommand(
            FullName: ExternalContactInput.FullName,
            Role: ExternalContactMapper.GetRoleByContactType(contactType),
            Email: ExternalContactInput.Email ?? string.Empty,
            PhoneNumber: ExternalContactInput.Phone ?? string.Empty,
            Category: ExternalContactMapper.MapContactTypeToCategory(contactType),
            IsPrimaryContact: ExternalContactInput.IsPrimaryProjectContact,
            ProjectId: new ProjectId(Guid.Parse(ProjectId)),
            EstablishmentUrn: null,
            OrganisationName: organisationName,
            LocalAuthorityId: null,
            Type: ContactType.Project
        );
    }

    private async Task<IActionResult> GetPageAsync()
    {
        var contactType = EnumExtensions.FromDescription<ExternalContactType>(this.SelectedExternalContactType);

        if (contactType == default)
        {
            var notFoundException = new Utils.Exceptions.NotFoundException(invalidContactTypeErrorMessage);

            logger.LogError(notFoundException, notFoundException.Message, notFoundException.InnerException);
            return NotFound();
        }

        await base.GetCurrentProjectAsync();
        return Page();
    }
}