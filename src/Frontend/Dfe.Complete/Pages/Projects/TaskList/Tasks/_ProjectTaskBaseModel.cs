using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks;

public class ProjectTaskBaseModel(ISender sender) : BaseProjectPageModel(sender)
{
    [BindProperty(SupportsGet = true, Name = "task_identifier")]
    [Required]
    public required string TaskIdentifier { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (TaskIdentifier != null)
        {
            var validTaskIdentifier = EnumExtensions.FromDescriptionValue<NoteTaskIdentifier>(TaskIdentifier);
            if (validTaskIdentifier == null)
                return NotFound();
        }

        var baseResult = await base.OnGetAsync();
        if (baseResult is not PageResult) return baseResult;

        return Page();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        return Redirect(string.Format(RouteConstants.ProjectTask, ProjectId, TaskIdentifier));
    }
}
