using Dfe.Complete.Constants;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation
{
    public class DaoRevocationModel(ISender sender, ILogger<DaoRevocationModel> logger, ICacheService<IMemoryCacheType> cacheService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await UpdateCurrentProject();
            await SetEstablishmentAsync();

            return Page();
        }

        public IActionResult OnPost()
        {
            return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectDaoRevocationConfirm));
        }
    }
}
