using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.StakeholderKickOffTask;

public class StakeholderKickOffTaskModel(ISender sender) : ProjectTaskModel(sender)
{
    [BindProperty]
    public string? TaskNote { get; set; }

    public override async Task<IActionResult> OnPostAsync()
    {
        // Handle stakeholder kick off specific form submission
        // Add validation logic here if needed
        
        return await base.OnPostAsync();
    }
}
