using Dfe.Complete.Application.ApiConfig;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Application.ApiAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class IgnoreApiInProductionAttribute : ApiExplorerSettingsAttribute
    {
        public IgnoreApiInProductionAttribute()
        {
            if (IgnoreApiInProductionConfig.IsProduction)
            {
                IgnoreApi = true;
            }
        }
    }
}
