using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.DateHistory
{
    public class DateHistoryProjectModel(ISender sender) : ProjectLayoutModel(sender, ConversionDateHistoryNavigation)
    {
        public override async Task<IActionResult> OnGet()
        {
            await base.OnGet();
            
            var projectWithHistories = new GetProjectHistoryByProjectIdQuery(ProjectId);
            var result = await sender.Send(projectWithHistories);
            
            Project = result.Value;
            
            return Page();
        }

        public (string, string) MapSignificantDateReason(ProjectDto project, SignificantDateHistoryReasonDto reason)
        {
            var notesBody = project.Notes
                .FirstOrDefault(n => n.NotableId == reason.Id?.Value)?.Body ?? "Unknown";

            var reasonType = EnumExtensions.FromDescription<SignificantDateReason>(reason.ReasonType);

            var label = SigDateHelper.MapLabel(reasonType);

            return (label, notesBody);
        }
    };
}
