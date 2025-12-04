using Dfe.Complete.Application.LocalAuthorities.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Models.ExternalContact;

public class ExternalContactAddEditPageModel(ITrustCache trustCacheService, ISender sender) : ExternalContactBasePageModel(sender, trustCacheService)
{
    private readonly ISender sender = sender;

    [BindProperty]
    public OtherExternalContactInputModel ExternalContactInput { get; set; } = new();

    public async virtual Task<IActionResult> OnGetAsync()
    {
        return await this.GetPage();
    }

    public async Task<string?> GetOrganisationNameAsync(ExternalContactType contactType)
    {
        if (Project == null)
            return string.Empty;

        return contactType switch
        {
            ExternalContactType.HeadTeacher or ExternalContactType.ChairOfGovernors or ExternalContactType.SchoolOrAcademy
                => Project.EstablishmentName?.ToTitleCase(),

            ExternalContactType.IncomingTrust
                => await GetIncomingTrustNameAsync(),

            ExternalContactType.OutgoingTrust
                => await GetOutgoingTrustNameAsync(),

            ExternalContactType.Solicitor
                => ExternalContactInput.OrganisationSolicitor,

            ExternalContactType.Diocese
                => ExternalContactInput.OrganisationDiocese,

            ExternalContactType.LocalAuthority
                => await GetLocalAuthorityNameAsync(),

            _ => ExternalContactInput.OrganisationOther
        };
    }    

    private async Task<string?> GetOutgoingTrustNameAsync()
    {
        if (Project?.Type != ProjectType.Transfer || Project.OutgoingTrustUkprn is null)
            return null;

        var trust = await trustCacheService.GetTrustAsync(Project.OutgoingTrustUkprn);
        return trust?.Name?.ToTitleCase();
    }

    private async Task<string?> GetLocalAuthorityNameAsync()
    {
        if (Project?.Urn is null)
            return null;

        var establishmentResult = await sender.Send(
            new GetEstablishmentByUrnRequest(Project.Urn.Value.ToString())
        );

        if (!establishmentResult.IsSuccess ||
            establishmentResult.Value?.LocalAuthorityCode is null or "")
        {
            return null;
        }

        var laResult = await sender.Send(
            new GetLocalAuthorityByCodeQuery(establishmentResult.Value.LocalAuthorityCode)
        );

        return laResult.IsSuccess ? laResult.Value?.Name ?? null : null;
    }

    private async Task<IActionResult> GetPage()
    {
        await this.GetCurrentProjectAsync();
        if (this.Project == null)
        {
            return NotFound();
        }

        this.SetExternalContactTypes();
        return Page();
    }

    private void SetExternalContactTypes()
    {
        var contactTypeRadioOptions = new List<ExternalContactType>
        {
                ExternalContactType.SchoolOrAcademy,
                ExternalContactType.IncomingTrust,
                ExternalContactType.OutgoingTrust,
                ExternalContactType.LocalAuthority,
                ExternalContactType.Solicitor,
                ExternalContactType.Diocese,
                ExternalContactType.Other,
        };

        if (this.Project?.Type == ProjectType.Conversion)
        {
            contactTypeRadioOptions.RemoveAll(x => x == ExternalContactType.OutgoingTrust);
        }

        this.ExternalContactInput.ContactTypeRadioOptions = contactTypeRadioOptions.ToArray();
    }
}
