using Dfe.Complete.Application.DaoRevoked.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.Reasons
{
    public class DaoRevocationReasonsModel(ISender sender, ILogger<DaoRevocationReasonsModel> logger, ErrorService errorService,
        ICacheService<IMemoryCacheType> cacheService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService)
    {
        public IFormCollection FormValues { get; set; } = default!;

        public List<DaoRevokedReason> Reasons { get; set; } = [];

        public Dictionary<DaoRevokedReason, string> ReasonNotes { get; private set; } = [];

        public override async Task<IActionResult> OnGetAsync()
        {
            PoplateOptions();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PoplateOptions();
            ValidateReasons();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdateCacheAsync(new RecordDaoRevocationDecisionCommand(new ProjectId(new Guid(ProjectId)))
            {
                ReasonNotes = ReasonNotes,
            });

            return RedirectToDaoRevocationRoute(RouteConstants.ProjectDaoRevocationMinister);
        }

        private void ValidateReasons()
        {
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
