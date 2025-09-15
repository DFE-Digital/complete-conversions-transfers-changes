using Dfe.Complete.Application.DaoRevoked.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.ProjectView;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation
{
    [ExcludeFromCodeCoverage]
    public abstract class DaoRevocationProjectLayoutModel(ISender sender, ILogger logger, ICacheService<IMemoryCacheType> cacheService) : ProjectLayoutModel(sender, logger, RecordDaoRevocationNavigation)
    {
        protected string CacheKey => $"DaoRevocation_{CacheKeyHelper.GenerateHashedCacheKey(ProjectId)}";

        protected async Task<RecordDaoRevocationDecisionCommand> GetCachedDecisionAsync()
        {
            return await cacheService.GetOrAddAsync(CacheKey, () =>
            {
                var command = new RecordDaoRevocationDecisionCommand(new ProjectId(new Guid(ProjectId)));
                return Task.FromResult(command);
            }, string.Empty);
        }

        protected async Task UpdateCacheAsync(RecordDaoRevocationDecisionCommand decison)
        { 
            cacheService.Remove(CacheKey); 
            await cacheService.GetOrAddAsync(CacheKey, () =>
            { 
                return Task.FromResult(decison);
            }, string.Empty);
        }
        public RedirectResult RedirectToDaoRevocationPage()
            => RedirectToDaoRevocationRoute(RouteConstants.ProjectDaoRevocation);

        public string GetDaoRevocationCheckPage()
            => FormatRouteWithProjectId(RouteConstants.ProjectDaoRevocationCheck);

        protected RedirectResult RedirectToDaoRevocationRoute(string route)
            => Redirect(FormatRouteWithProjectId(route));

    }
}
