using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.DateHistory
{
    public class ChangeReasonProjectModel(ISender sender, ErrorService errorService, ILogger<ChangeReasonProjectModel> logger) : ProjectLayoutModel(sender, logger, ConversionDateHistoryNavigation)
    {
        [BindProperty]
        public string SignificantDateString { get; set; }
        
        public DateOnly SignificantDate { get; set; }
        
        [BindProperty]
        public IFormCollection FormValues { get; set; } = default!;

        public List<SignificantDateReason> Reasons  { get; set; }

        public Dictionary<SignificantDateReason, string> ReasonNotes { get; private set; } = new();

        public override async Task<IActionResult> OnGetAsync()
        {
            await PopulatePage();
            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {
            await PopulatePage();

            foreach (var reason in Reasons)
            {
                var reasonKey = $"date_history_reasons[{reason.ToDescription()}]";
                var noteKey = $"date_history_reasons[{reason.ToDescription()}_note]";

                var isChecked = FormValues.TryGetValue(reasonKey, out var selected) && selected == "1";

                if (!isChecked)
                    continue;

                var hasNote = FormValues.TryGetValue(noteKey, out var note);
                var isNoteValid = hasNote && !string.IsNullOrWhiteSpace(note);

                if (isNoteValid)
                {
                    ReasonNotes[reason] = note!;
                }
                else
                {
                    ModelState.AddModelError($"{reason.ToDescription()}_note", "Enter details about the reason for the change.");
                    errorService.AddErrors(ModelState);
                }
            }

            if (ReasonNotes.Count == 0)
            {
                ModelState.AddModelError(nameof(SignificantDateString), "You must choose at least one reason");
                errorService.AddErrors(ModelState);
            }

            if (!ModelState.IsValid)
                return Page();
            
            var command = new UpdateSignificantDateCommand(Project.Id, DateOnly.Parse(SignificantDateString), ReasonNotes, User.Identity?.Name);
            await sender.Send(command);
            
            return Redirect(FormatRouteWithProjectId(RouteConstants.ChangeProjectDateHistoryConfirm));
        }

        private async Task PopulatePage()
        {
            await base.OnGetAsync();
            
            var previousDate = Project.SignificantDate;
            var dateString = TempData["SignificantDate"]?.ToString() ?? SignificantDateString;
            
            SignificantDate = DateOnly.Parse(dateString);

            if (SignificantDate > previousDate)
            {
                Reasons = new()
                {
                    SignificantDateReason.IncomingTrust,
                    SignificantDateReason.School,
                    SignificantDateReason.LocalAuthority,
                    SignificantDateReason.Diocese,
                    SignificantDateReason.Tupe,
                    SignificantDateReason.Pensions,
                    SignificantDateReason.Union,
                    SignificantDateReason.NegativePress,
                    SignificantDateReason.Governance,
                    SignificantDateReason.Finance,
                    SignificantDateReason.Viability,
                    SignificantDateReason.Land,
                    SignificantDateReason.Buildings,
                    SignificantDateReason.LegalDocuments,
                    SignificantDateReason.CorrectingAnError,
                    SignificantDateReason.VoluntaryDeferral,
                    SignificantDateReason.AdvisoryBoardConditions,
                };
            }
            else
            {
                Reasons = new()
                {
                    SignificantDateReason.ProgressingFasterThanExpected,
                    SignificantDateReason.CorrectingAnError
                };
            }
        }
    }
}