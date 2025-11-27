using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.DateHistory
{
    public class DateHistoryProjectModel(ISender sender, ILogger<DateHistoryProjectModel> logger, IProjectPermissionService projectPermissionService) : ProjectLayoutModel(sender, logger, projectPermissionService, ConversionDateHistoryNavigation)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            var projectWithHistories = new GetProjectHistoryByProjectIdQuery(ProjectId);
            var result = await Sender.Send(projectWithHistories);

            Project = result.Value!;

            return Page();
        }

        public (string, string) MapSignificantDateReason(ProjectDto project, SignificantDateHistoryReasonDto reason)
        {
            var notesBody = project.Notes
                .FirstOrDefault(n => n.NotableId == reason.Id?.Value)?.Body ?? "Unknown";

            var reasonType = EnumExtensions.FromDescription<SignificantDateReason>(reason.ReasonType);

            var label = reasonType.ToDisplayDescription();

            return (label, notesBody);
        }
    };
}
