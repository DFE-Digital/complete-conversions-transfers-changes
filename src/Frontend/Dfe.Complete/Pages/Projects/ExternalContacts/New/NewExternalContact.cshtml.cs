using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ExternalContacts.New;

public class NewExternalContact(ISender sender, ILogger<NewExternalContact> logger)
    : ExternalContactBasePageModel(sender, logger)
{
    public ExternalContactType[] ContactTypeRadioOptions { get; set; }

    [BindProperty]
    public string? SelectedExternalContactType { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();
        this.SetExternalContactTypes();
        return Page();
    }

    public IActionResult OnPost()
    {
        var contactType = EnumExtensions.FromDescription<ExternalContactType>(this.SelectedExternalContactType);

        var pageToRedirectTo = contactType switch
        {
            ExternalContactType.SomeOneElse => string.Format(RouteConstants.ProjectsExternalContactAddTypeOther, ProjectId),
            ExternalContactType.HeadTeacher or ExternalContactType.IncomingTrustCEO or ExternalContactType.OutgoingTrustCEO or ExternalContactType.ChairOfGovernors
                => string.Format(RouteConstants.ProjectsExternalContactAdd, ProjectId, this.SelectedExternalContactType),
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(pageToRedirectTo))
        {
            var error = $"The selected contact type '{this.SelectedExternalContactType}' is invalid.";
            var notFoundException = new Utils.NotFoundException(error);

            logger.LogError(notFoundException, notFoundException.Message, notFoundException.InnerException);
            throw notFoundException;
        }

        return Redirect(pageToRedirectTo);
    }

    private void SetExternalContactTypes()
    {
        if (this.Project?.Type == ProjectType.Transfer)
        {
            this.ContactTypeRadioOptions = new[]
            {
                ExternalContactType.HeadTeacher,
                ExternalContactType.IncomingTrustCEO,
                ExternalContactType.OutgoingTrustCEO,
                ExternalContactType.SomeOneElse,
            };
        }
        else
        {
            this.ContactTypeRadioOptions = new[]
            {
                ExternalContactType.HeadTeacher,
                ExternalContactType.IncomingTrustCEO,
                ExternalContactType.ChairOfGovernors,
                ExternalContactType.SomeOneElse,
            };
        }
    }
}