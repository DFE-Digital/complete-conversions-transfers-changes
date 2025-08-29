using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Domain.Enums;
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
            return RedirectToPage("../ExternalContacts", new { ProjectId });
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