using Dfe.Complete.Constants;
using Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.MinisterName;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.Date
{
    public class ChangeDaoRevocationDecisionDateModel(ISender sender, ILogger<AddDaoRevocationMinisterNameModel> logger, IErrorService errorService,
        ICacheService<IMemoryCacheType> cacheService, IProjectPermissionService projectPermissionService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService, projectPermissionService)
    {
        [BindProperty]
        [Required(ErrorMessage = ValidationConstants.DecisionDateRequired)]
        public DateOnly? Date { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            var decision = await GetCachedDecisionAsync();

            if (decision.ReasonNotes?.Count == 0)
                return RedirectToDaoRevocationPage();

            Date = decision.DecisionDate;
            return ReturnPage(decision);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            }

            var command = await GetCachedDecisionAsync();

            command.DecisionDate = Date;
            await UpdateCacheAsync(command);

            return RedirectToDaoRevocationRoute(RouteConstants.ProjectDaoRevocationCheck);
        }
    }
}

