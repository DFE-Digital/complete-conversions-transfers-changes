using Dfe.Complete.Constants;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation
{
    public class ConfirmDaoRevocationModel(ISender sender, ILogger<ConfirmDaoRevocationModel> logger, ICacheService<IMemoryCacheType> cacheService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService)
    {
        [BindProperty]
        public bool? Approval { get; set; }

        [BindProperty]
        public bool? Sent { get; set; }

        [BindProperty]
        public bool? Saved { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!(Approval == true && Sent == true && Saved == true))
            {
                TempData["ConfirmNotification"] = true;
                return Page();
            }
            return Redirect(FormatRouteWithProjectId(RouteConstants.ProjectDaoRevocationReason));
        }
    }
}
