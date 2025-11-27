using Dfe.Complete.Constants;
using Dfe.Complete.Services;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation
{
    public class DaoRevocationModel(ISender sender, ILogger<DaoRevocationModel> logger, ICacheService<IMemoryCacheType> cacheService, IProjectPermissionService projectPermissionService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService, projectPermissionService)
    {
        public override async Task<IActionResult> OnGetAsync()
        {
            await UpdateCurrentProject();

            var permissionResult = await CheckDaoRevocationPermissionAsync();
            if (permissionResult != null) return permissionResult;

            await SetEstablishmentAsync();

            return Page();
        }

        public IActionResult OnPost()
        {
            return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectDaoRevocationConfirm));
        }
    }
}
