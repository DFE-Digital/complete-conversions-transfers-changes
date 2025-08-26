using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Infrastructure.Gateways;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;

namespace Dfe.Complete.StartupConfiguration
{
    public static class DependencyConfigurationExtensions
    {
        public static IServiceCollection AddClientDependencies(this IServiceCollection services)
        {
            services.AddScoped<IErrorService, ErrorService>();
            services.AddScoped<CompleteApiClient, CompleteApiClient>();
            services.AddScoped<IAnalyticsConsentService, AnalyticsConsentService>();
            services.AddScoped<ITrustCache, TrustCacheService>();

            return services;
        }
    }
}
