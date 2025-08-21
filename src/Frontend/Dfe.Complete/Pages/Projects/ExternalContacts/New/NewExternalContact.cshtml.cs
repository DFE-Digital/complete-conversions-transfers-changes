using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ExternalContacts.New;

public class NewExternalContact(IProjectService projectService, ILogger<NewExternalContact> logger)
    : ExternalContactBasePageModel(projectService, logger)
{
    public ExternalContactType[] ContactTypeRadioOptions { get; set; }

    [BindProperty(SupportsGet = true, Name = "SelectedExternalContactType")]
    public string? SelectedExternalContactType { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();
        this.SetExternalContactTypes();

        return Page();
    }

    public IActionResult OnPost()
    {
        var pageToRedirectTo = EnumExtensions.FromDescription<ExternalContactType>(this.SelectedExternalContactType) switch
        {
            ExternalContactType .Other => string.Format(RouteConstants.ProjectsExternalContactAddTypeOther, ProjectId),
            _ => string.Format(RouteConstants.ProjectsExternalContactAdd, ProjectId, SelectedExternalContactType)
        };

        return Redirect(pageToRedirectTo);
    }

    private void SetExternalContactTypes()
    {
        if(this.Project?.Type == ProjectType.Transfer)
        {
           this.ContactTypeRadioOptions = new[]
           {
                ExternalContactType.HeadTeacher,
                ExternalContactType.IncomingTrustCEO,
                ExternalContactType.OutgoingTrustCEO,
                ExternalContactType.Other,
            };
        }
        else
        {
            this.ContactTypeRadioOptions = new[]
            {
                ExternalContactType.HeadTeacher,
                ExternalContactType.IncomingTrustCEO,
                ExternalContactType.ChairOfGovernors,
                ExternalContactType.Other,
            };
        }

        if(string.IsNullOrWhiteSpace(this.SelectedExternalContactType)) 
            this.SelectedExternalContactType = EnumExtensions.ToDescription<ExternalContactType>(ExternalContactType.Other);
    }
}