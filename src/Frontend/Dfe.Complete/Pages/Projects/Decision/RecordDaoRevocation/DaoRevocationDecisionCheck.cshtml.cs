using Dfe.Complete.Application.DaoRevoked.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.MinisterName;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation
{
    public class DaoRevocationCheckModel(ISender sender, ILogger<AddDaoRevocationMinisterNameModel> logger,
        ICacheService<IMemoryCacheType> cacheService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService)
    {
        [BindProperty]
        public RecordDaoRevocationDecisionCommand? Decision { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            var decision = await GetCachedDecisionAsync();

            if (decision.ReasonNotes?.Count == 0)
                return RedirectToDaoRevocationPage();

            Decision = decision;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var decision = await GetCachedDecisionAsync();
            decision.UserId = User.GetUserId();
            await sender.Send(decision);

            cacheService.Remove(CacheKey);
            TempData["RecordedDaoRevocation"] = true;
            return Redirect(FormatRouteWithProjectId(RouteConstants.Project));
        }
    }
}
