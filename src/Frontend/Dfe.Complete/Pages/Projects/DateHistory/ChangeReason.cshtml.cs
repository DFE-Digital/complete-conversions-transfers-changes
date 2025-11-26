using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.DateHistory
{
    public class ChangeReasonProjectModel(ISender sender, IErrorService errorService, ILogger<ChangeReasonProjectModel> logger, IProjectPermissionService projectPermissionService) : ProjectLayoutModel(sender, logger, projectPermissionService, ConversionDateHistoryNavigation)
    {
        [BindProperty]
        public string SignificantDateString { get; set; }

        public DateOnly SignificantDate { get; set; }

        [BindProperty]
        public IFormCollection FormValues { get; set; } = default!;

        public List<SignificantDateReason> Reasons { get; set; }

        public Dictionary<SignificantDateReason, string> ReasonNotes { get; private set; } = new();

        public override async Task<IActionResult> OnGetAsync()
        {
            await PopulatePage();

            if (!CanEditSignificantDate)
            {
                TempData.SetNotification(
                    NotificationType.Error,
                    "Important",
                    "You are not authorised to perform this action."
                );
                return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectTaskList));
            }

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

            var command = new UpdateSignificantDateCommand(Project.Id, DateOnly.Parse(SignificantDateString), ReasonNotes, User.GetUserId());
            await Sender.Send(command);

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
                var reasons = new List<SignificantDateReason>();

                reasons.Add(SignificantDateReason.IncomingTrust);
                if (Project.Type == ProjectType.Transfer)
                {
                    reasons.Add(SignificantDateReason.OutgoingTrust);
                }
                if (Project.Type == ProjectType.Conversion)
                {
                    reasons.Add(SignificantDateReason.School);
                }
                else
                {
                    reasons.Add(SignificantDateReason.Academy);
                }

                reasons.Add(SignificantDateReason.LocalAuthority);
                reasons.Add(SignificantDateReason.Diocese);
                reasons.Add(SignificantDateReason.Tupe);
                reasons.Add(SignificantDateReason.Pensions);
                reasons.Add(SignificantDateReason.Union);
                reasons.Add(SignificantDateReason.NegativePress);
                reasons.Add(SignificantDateReason.Governance);
                reasons.Add(SignificantDateReason.Finance);
                reasons.Add(SignificantDateReason.Viability);
                reasons.Add(SignificantDateReason.Land);
                reasons.Add(SignificantDateReason.Buildings);
                reasons.Add(SignificantDateReason.LegalDocuments);
                if (Project.Type == ProjectType.Transfer)
                {
                    reasons.Add(SignificantDateReason.CommercialTransferAgreement);
                }
                reasons.Add(SignificantDateReason.CorrectingAnError);
                reasons.Add(SignificantDateReason.VoluntaryDeferral);
                reasons.Add(SignificantDateReason.AdvisoryBoardConditions);

                Reasons = reasons;
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