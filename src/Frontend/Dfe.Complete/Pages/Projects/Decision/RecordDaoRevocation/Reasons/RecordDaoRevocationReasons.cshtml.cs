using Dfe.Complete.Application.DaoRevoked.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation.Reasons
{
    public class DaoRevocationReasonsModel(ISender sender, ILogger<DaoRevocationReasonsModel> logger, IErrorService errorService,
        ICacheService<IMemoryCacheType> cacheService, IProjectPermissionService projectPermissionService) : DaoRevocationProjectLayoutModel(sender, logger, cacheService, projectPermissionService)
    {
        public IFormCollection FormValues { get; set; } = default!;

        public List<DaoRevokedReason> Reasons { get; set; } = [];

        public Dictionary<DaoRevokedReason, string> ReasonNotes { get; private set; } = [];

        public override async Task<IActionResult> OnGetAsync()
        {
            PopulateOptions(Reasons);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PopulateOptions(Reasons);
            ValidateReasons(FormValues, Reasons, ReasonNotes, errorService, ModelState);

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
    }
}
