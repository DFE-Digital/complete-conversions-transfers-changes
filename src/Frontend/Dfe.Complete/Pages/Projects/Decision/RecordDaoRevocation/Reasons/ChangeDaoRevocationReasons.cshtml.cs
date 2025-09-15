using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.Reasons
{
    public class ChangeDaoRevocationReasonsModel(ISender sender, ILogger<DaoRevocationReasonsModel> logger, ErrorService errorService,
        ICacheService<IMemoryCacheType> cacheService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService)
    {
        public Dictionary<string, string> FormValues { get; set; } = []; 

        public List<DaoRevokedReason> Reasons { get; set; } = [];

        public Dictionary<DaoRevokedReason, string> ReasonNotes { get; private set; } = [];

        public override async Task<IActionResult> OnGetAsync()
        {
            PoplateOptions();

            var command = await GetCachedDecisionAsync();

            ReasonNotes = command.ReasonNotes ?? [];
            foreach (var kvp in ReasonNotes)
            {
                var reason = kvp.Key;
                var note = kvp.Value;

                var name = reason.ToDescription();
                var noteKey = $"{name}_note";

                // Mark as checked
                FormValues[$"dao_revoked_reasons[{name}]"] = "1";

                // Add note if present
                if (!string.IsNullOrWhiteSpace(note))
                {
                    FormValues[$"dao_revoked_reasons[{noteKey}]"] = note;
                }
            }

            return command.ReasonNotes?.Count == 0 ? RedirectToDaoRevocationPage() : Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PoplateOptions();
            ValidateReasons();

            if (!ModelState.IsValid)
            {
                return Page();
            } 

            var command = await GetCachedDecisionAsync();

            command.ReasonNotes = ReasonNotes;

            await UpdateCacheAsync(command);

            return RedirectToDaoRevocationRoute(RouteConstants.ProjectDaoRevocationCheck);
        }

        private void ValidateReasons()
        {
            FormValues = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());

            foreach (var reason in Reasons)
            {
                var reasonKey = $"dao_revoked_reasons[{reason.ToDescription()}]";
                var noteKey = $"dao_revoked_reasons[{reason.ToDescription()}_note]";

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
                    ModelState.AddModelError($"{reason.ToDescription()}_note", ValidationConstants.MustProvideDetails);
                    errorService.AddErrors(ModelState);
                }
            }

            if (ReasonNotes.Count == 0 && !errorService.HasErrors())
            {
                ModelState.AddModelError("select-dao-revoked-reason", ValidationConstants.ChooseAtLeastOneReason);
                errorService.AddErrors(ModelState);
            }
        }
        private void PoplateOptions()
        {
            Reasons.Add(DaoRevokedReason.SchoolRatedGoodOrOutstanding);
            Reasons.Add(DaoRevokedReason.SafeguardingConcernsAddressed);
            Reasons.Add(DaoRevokedReason.SchoolClosedOrClosing);
            Reasons.Add(DaoRevokedReason.ChangeToGovernmentPolicy);
        }
    }
}
