using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ExternalContacts.New;

public class CreateExternalContact(ErrorService errorService, IProjectService projectService, ILogger<CreateExternalContact> logger)
    : ExternalContactBasePageModel(projectService, logger)
{

    [FromQuery(Name = "externalcontacttype")]
    public string? SelectedExternalContactType { get; set; }

    [BindProperty]
    public ExternalContactDetailsModel ExternalContactDetails { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {   
        await base.OnGetAsync();

        if (string.IsNullOrWhiteSpace(SelectedExternalContactType))
        {
            this.SelectedExternalContactType = ExternalContactType.Other.ToDescription();
        }
        return Page();
    }
    
    //public async Task<IActionResult> OnPostAsync()
    //{
    //    //var pageToRedirectTo =  (EnumExtensions.FromDescription<ExternalContactType>(this.ContactType)) switch
    //    //{
    //    //    "conversion" => "/Projects/Conversion/CreateNewProject",
    //    //    "fam_conversion" => "/Projects/MatConversion/CreateNewProject",
    //    //    "transfer" => "/Projects/Transfer/CreateNewProject",
    //    //    "fam_transfer" => "/Projects/MatTransfer/CreateNewProject",
    //    //    _ => string.Empty
    //    //};

    //    return Page();
    //}

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            errorService.AddErrors(ModelState);
            return Page();
        }
        return Page();
    }
}