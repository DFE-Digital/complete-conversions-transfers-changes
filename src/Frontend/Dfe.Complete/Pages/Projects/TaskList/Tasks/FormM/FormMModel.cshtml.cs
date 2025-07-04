using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Constants;
using Dfe.Complete.Application.Projects.Commands.TransferTasks;
using Dfe.Complete.Application.Projects.Queries.TransferTasks;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.FormM;

public class FormMModel(ISender sender, IAuthorizationService authorizationService) : ProjectTaskModel(sender, authorizationService)
{
    [BindProperty(Name = "not-applicable")]
    public bool NotApplicable { get; set; }

    [BindProperty(Name = "received-form-m")]
    public bool ReceivedFormM { get; set; }

    [BindProperty(Name = "received-title-plans")]
    public bool ReceivedTitlePlans { get; set; }

    [BindProperty(Name = "cleared")]
    public bool Cleared { get; set; }

    [BindProperty(Name = "signed")]
    public bool Signed { get; set; }

    [BindProperty(Name = "saved")]
    public bool Saved { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        TaskIdentifier = "form_m";
        Title = "Form M";
        await base.OnGetAsync();
        if (Project.Type != ProjectType.Transfer)
            return NotFound();

        var result = await Sender.Send(new GetTransferFormMTaskDataByProjectIdQuery(new ProjectId(new Guid(ProjectId))));
        if (!result.IsSuccess || result.Value == null)
            throw new InvalidOperationException($"Failed to retrieve Form M task data: {result.Error}");

        var formMData = result.Value;
        NotApplicable = formMData.FormMNotApplicable ?? false;
        ReceivedFormM = formMData.FormMReceivedFormM ?? false;
        ReceivedTitlePlans = formMData.FormMReceivedTitlePlans ?? false;
        Cleared = formMData.FormMCleared ?? false;
        Signed = formMData.FormMSigned ?? false;
        Saved = formMData.FormMSaved ?? false;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        TaskIdentifier = "form_m";
        await base.OnGetAsync();

        var command = new UpdateTransferFormMTaskDataByProjectIdCommand(
            new ProjectId(new Guid(ProjectId)),
            NotApplicable,
            ReceivedFormM,
            ReceivedTitlePlans,
            Cleared,
            Signed,
            Saved
        );
        await Sender.Send(command);
        return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
    }
}