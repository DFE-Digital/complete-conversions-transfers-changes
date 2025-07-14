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
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            
            var projectWithHistories = new GetProjectHistoryByProjectIdQuery(ProjectId);
            var result = await sender.Send(projectWithHistories);
            
            Project = result.Value!;
            
            return Page();
        }


        public (string, string) MapSignificantDateReason(ProjectDto project, SignificantDateHistoryReasonDto reason)
        {
            var notesBody = project.Notes
                .FirstOrDefault(n => n.NotableId == reason.Id.Value)?.Body ?? "Unknown";

            var reasonType = EnumExtensions.FromDescription<SignificantDateReason>(reason.ReasonType);

            var reasonLabels = new Dictionary<SignificantDateReason, string>
            {
                [SignificantDateReason.AdvisoryBoardConditions] = "AdvisoryBoardConditions",
                [SignificantDateReason.Buildings] = "Buildings",
                [SignificantDateReason.CorrectingAnError] = "Correcting an error",
                [SignificantDateReason.Diocese] = "Diocese",
                [SignificantDateReason.Finance] = "Finance",
                [SignificantDateReason.Governance] = "Governance",
                [SignificantDateReason.IncomingTrust] = "Incoming trust",
                [SignificantDateReason.Land] = "Land",
                [SignificantDateReason.LegacyReason] = "Legacy reason, see note",
                [SignificantDateReason.LegalDocuments] = "Legal Documents",
                [SignificantDateReason.LocalAuthority] = "Local Authority",
                [SignificantDateReason.NegativePress] = "Negative press",
                [SignificantDateReason.OutgoingTrust] = "Outgoing trust",
                [SignificantDateReason.Pensions] = "Pensions",
                [SignificantDateReason.ProgressingFasterThanExpected] = "Project is progressing faster than expected",
                [SignificantDateReason.School] = "School",
                [SignificantDateReason.StakeholderKickOff] = "Confirmed as part of the stakeholder kick off task",
                [SignificantDateReason.Tupe] = "Tupe",
                [SignificantDateReason.Union] = "Union",
                [SignificantDateReason.Viability] = "Viability",
                [SignificantDateReason.VoluntaryDeferral] = "VoluntaryDeferral"
            };

            var label = reasonLabels.TryGetValue(reasonType, out var name) ? name : "Unknown";

            return (label, notesBody);
        }
    };
}
