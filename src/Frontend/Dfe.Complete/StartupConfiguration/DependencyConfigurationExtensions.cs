using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Infrastructure.Gateways;
using Dfe.Complete.Infrastructure.Repositories;
using Dfe.Complete.Services;

namespace Dfe.Complete.StartupConfiguration
{
    public static class DependencyConfigurationExtensions
    {
        public static IServiceCollection AddClientDependencies(this IServiceCollection services)
        {
            services.AddScoped<IErrorService, ErrorService>();
            services.AddScoped<CompleteApiClient, CompleteApiClient>();
            services.AddScoped<AcademiesApiClient, AcademiesApiClient>();

            services.AddScoped<IAnalyticsConsentService, AnalyticsConsentService>();
            
            //            services.AddScoped(typeof(ICompleteRepository<>), typeof(CompleteRepository<>));
            return services;
        }
    }
}
