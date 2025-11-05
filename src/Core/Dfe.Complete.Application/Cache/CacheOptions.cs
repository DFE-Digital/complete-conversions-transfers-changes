using Microsoft.Extensions.Caching.Distributed;

namespace Dfe.Complete.Application.Cache
{
    public static class CacheOptions
    {
        public static readonly DistributedCacheEntryOptions DefaultCacheOptions = new DistributedCacheEntryOptions()
             .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
    }
}
