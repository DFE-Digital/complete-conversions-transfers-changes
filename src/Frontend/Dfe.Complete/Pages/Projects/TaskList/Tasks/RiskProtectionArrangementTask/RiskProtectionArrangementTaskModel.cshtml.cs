using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.RiskProtectionArrangementTask;

public class RiskProtectionArrangementTaskModel(ISender sender) : ProjectTaskModel(sender)
{
    [BindProperty]
    public string? RiskAssessment { get; set; }

    [BindProperty]
    public bool? ArrangementConfirmed { get; set; }

    public override async Task<IActionResult> OnPostAsync()
    {
        // Handle risk protection arrangement specific form submission
        // Add validation logic here if needed
        
        return await base.OnPostAsync();
    }
}
