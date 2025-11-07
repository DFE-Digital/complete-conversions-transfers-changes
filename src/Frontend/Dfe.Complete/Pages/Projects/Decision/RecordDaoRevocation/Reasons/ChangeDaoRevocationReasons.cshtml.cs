using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.Reasons
{
    public class ChangeDaoRevocationReasonsModel(ISender sender, ILogger<DaoRevocationReasonsModel> logger, IErrorService errorService,
        ICacheService<IMemoryCacheType> cacheService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService)
    {
        public Dictionary<string, string> FormValues { get; set; } = []; 

        public List<DaoRevokedReason> Reasons { get; set; } = [];

        public Dictionary<DaoRevokedReason, string> ReasonNotes { get; private set; } = [];

        public override async Task<IActionResult> OnGetAsync()
        {
            PoplateOptions(Reasons);

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
            PoplateOptions(Reasons);
            ValidateReasons(Request.Form, Reasons, ReasonNotes, errorService, ModelState);
            
            FormValues = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString()); 

            if (!ModelState.IsValid)
            {
                return Page();
            } 

            var command = await GetCachedDecisionAsync();

            command.ReasonNotes = ReasonNotes;

            await UpdateCacheAsync(command);

            return RedirectToDaoRevocationRoute(RouteConstants.ProjectDaoRevocationCheck);
        } 
    }
}
