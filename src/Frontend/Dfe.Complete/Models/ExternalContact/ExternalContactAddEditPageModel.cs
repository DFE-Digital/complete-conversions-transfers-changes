using Dfe.Complete.Application.LocalAuthorities.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Models.ExternalContact;

public class ExternalContactAddEditPageModel(ITrustCache trustCacheService, ISender sender, ILogger logger) : ExternalContactBasePageModel(sender, logger)
{   

    [BindProperty]
    public OtherExternalContactInputModel ExternalContactInput { get; set; } = new();

    public async virtual Task<IActionResult> OnGetAsync()
    {   
        return await this.GetPage();
    }

    protected async Task<string?> GetOrganisationName(ExternalContactType contactType)
    {
        var organisationName = string.Empty;

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
                var establishmentResult = await sender.Send(establishmentQuery);

                if (establishmentResult.IsSuccess && establishmentResult.Value != null)
                {
                    var laQuery = new GetLocalAuthorityByCodeQuery(establishmentResult.Value?.LocalAuthorityCode);

                    var la = await sender.Send(laQuery);
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

        return organisationName;
    }

    private async Task<IActionResult> GetPage()
    {   
        await this.GetCurrentProject();
        if (this.Project == null)
        {
            return NotFound();
        }

        this.SetExternalContactTypes();
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

        //if (string.IsNullOrWhiteSpace(this.ExternalContactInput.SelectedExternalContactType))
        //{
        //    this.ExternalContactInput.SelectedExternalContactType = ExternalContactType.Other.ToDescription();
        //}
    }   
}
