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
public class CreateOtherExternalContact(
    IValidator<OtherExternalContactInputModel> otherExternalContactInputModelValidator,
    ITrustCache trustCacheService, IErrorService errorService,
    ISender sender,
    ILogger<CreateOtherExternalContact> logger)
    : ExternalContactAddEditPageModel(trustCacheService, sender)
{
    private readonly IErrorService errorService = errorService;
    private readonly ISender sender = sender;
    private readonly ILogger<CreateOtherExternalContact> logger = logger;
    private readonly IValidator<OtherExternalContactInputModel> validator = otherExternalContactInputModelValidator;

    public async override Task<IActionResult> OnGetAsync()
    {
        return await this.GetPage();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Use FluentValidation.Results.ValidationResult instead of System.ComponentModel.DataAnnotations.ValidationResult
        FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(ExternalContactInput);

        if (!validationResult.IsValid)
        {
            AddValidationErrorsToModelState(validationResult);
            errorService.AddErrors(ModelState);
            return await this.GetPage();
        }
        else
        {
            try
            {
                var contactType = EnumExtensions.FromDescription<ExternalContactType>(this.ExternalContactInput.SelectedExternalContactType);

                await this.GetCurrentProjectAsync();

                var category = ExternalContactMapper.MapContactTypeToCategory(contactType);
                var organisationName = await this.GetOrganisationNameAsync(contactType);

                var newExternalContactCommand = new CreateExternalContactCommand(
                    FullName: this.ExternalContactInput.FullName,
                    Role: this.ExternalContactInput.Role,
                    Email: this.ExternalContactInput.Email ?? string.Empty,
                    PhoneNumber: this.ExternalContactInput.Phone ?? string.Empty,
                    Category: category,
                    IsPrimaryContact: this.ExternalContactInput.IsPrimaryProjectContact,
                    ProjectId: new ProjectId(Guid.Parse(this.ProjectId!)),
                    EstablishmentUrn: null,
                    OrganisationName: organisationName,
                    LocalAuthorityId: null,
                    Type: ContactType.Project
                );

                var response = await sender.Send(newExternalContactCommand);
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
