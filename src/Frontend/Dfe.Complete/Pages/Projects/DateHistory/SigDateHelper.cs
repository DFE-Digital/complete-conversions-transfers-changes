using System.Globalization;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Pages.Projects.DateHistory;

public static class SigDateHelper
{
    private static readonly Dictionary<SignificantDateReason, string> ReasonLabelMappings = new()
    {
        [SignificantDateReason.AdvisoryBoardConditions] = "Advisory board conditions not met",
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
        [SignificantDateReason.Tupe] = "TuPE (Transfer of Undertakings Protection Employment rights)",
        [SignificantDateReason.Union] = "Union",
        [SignificantDateReason.Viability] = "Viability",
        [SignificantDateReason.VoluntaryDeferral] = "Voluntary deferral"
    };

    public static string MapLabel(SignificantDateReason reason)
    {
        return ReasonLabelMappings.TryGetValue(reason, out var name) ? name : "Unknown";
    }
    
    public static string CapitliseFirstLetter(string input)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
    }
}