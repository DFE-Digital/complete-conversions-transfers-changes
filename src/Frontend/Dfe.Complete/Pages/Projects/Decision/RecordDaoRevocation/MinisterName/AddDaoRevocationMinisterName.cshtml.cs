using Dfe.Complete.Constants;
using Dfe.Complete.Services;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.MinisterName
{
    public class AddDaoRevocationMinisterNameModel(ISender sender, ILogger<AddDaoRevocationMinisterNameModel> logger, ErrorService errorService,
        ICacheService<IMemoryCacheType> cacheService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService)
    {
        [BindProperty]
        [Required(ErrorMessage = ValidationConstants.MinisterNameRequired)]
        public required string Name { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            } 

            var command = await GetCachedDecisionAsync();

            command.MinisterName = Name;
            await UpdateCacheAsync(command);

            return RedirectToDaoRevocationRoute(RouteConstants.ProjectDaoRevocationDate);
        }
    }
}
