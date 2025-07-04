using System.Threading.Tasks;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Pages.Projects;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.FormM;

public class FormMModel : ProjectTaskModel
{
    private readonly ISender _sender;
    private readonly IAuthorizationService _authorizationService;

    public FormMModel(ISender sender, IAuthorizationService authorizationService)
        : base(sender, authorizationService)
    {
        _sender = sender;
        _authorizationService = authorizationService;
    }
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
        {
            return NotFound();
        }
        var result = await _sender.Send(new GetTransferFormMTaskDataByProjectIdQuery(new ProjectId(new Guid(ProjectId))));
        if (result is not null)
        {
            NotApplicable = result.FormMNotApplicable ?? false;
            ReceivedFormM = result.FormMReceivedFormM ?? false;
            ReceivedTitlePlans = result.FormMReceivedTitlePlans ?? false;
            Cleared = result.FormMCleared ?? false;
            Signed = result.FormMSigned ?? false;
            Saved = result.FormMSaved ?? false;
        }
        return Page();
    }

public async Task<IActionResult> OnPostAsync()
    {
        TaskIdentifier = "form_m";
        await base.OnGetAsync();
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var command = new UpdateTransferFormMTaskDataByProjectIdCommand(
            new ProjectId(new Guid(ProjectId)),
            NotApplicable,
            ReceivedFormM,
            ReceivedTitlePlans,
            Cleared,
            Signed,
            Saved
        );
        await _sender.Send(command);
        return RedirectToPage("../TaskList", new { projectId = ProjectId });
    }

    // Query/Command/DTOs in same file as handler per requirements
    public record GetTransferFormMTaskDataByProjectIdQuery(ProjectId ProjectId) : IRequest<TransferFormMTaskDataDto>;
    public record UpdateTransferFormMTaskDataByProjectIdCommand(ProjectId ProjectId, bool NotApplicable, bool ReceivedFormM, bool ReceivedTitlePlans, bool Cleared, bool Signed, bool Saved) : IRequest;
    public record TransferFormMTaskDataDto(bool? FormMNotApplicable, bool? FormMReceivedFormM, bool? FormMReceivedTitlePlans, bool? FormMCleared, bool? FormMSigned, bool? FormMSaved);
}
