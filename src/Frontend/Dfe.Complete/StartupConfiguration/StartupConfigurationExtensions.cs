using Dfe.AcademiesApi.Client;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.TramsDataApi.Client.Extensions;

namespace Dfe.Complete.StartupConfiguration
{
    public static class StartupConfigurationExtensions
    {
        public static IServiceCollection AddCompleteClientProject(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddClientDependencies();
            services.AddAcademiesApiClient<ITrustsV4Client, TrustsV4Client>(configuration);

            return services;
        }
    }
}
