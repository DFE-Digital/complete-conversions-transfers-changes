using Dfe.Complete.Constants;
using Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.MinisterName;
using Dfe.Complete.Services;
using DfE.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.Date
{
    public class ChangeDaoRevocationDecisionDateModel(ISender sender, ILogger<AddDaoRevocationMinisterNameModel> logger, ErrorService errorService,
        ICacheService<IMemoryCacheType> cacheService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService)
    {
        [BindProperty]
        [Required(ErrorMessage = ValidationConstants.DecisionDateRequired)]
        public DateOnly? Date { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            var command = await GetCachedDecisionAsync();

            if (command.ReasonNotes?.Count == 0)
                return RedirectToDaoRevocationPage();
             
            Date = command.DecisionDate;
            return command.ReasonNotes?.Count == 0 ? RedirectToDaoRevocationPage() : Page();
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

