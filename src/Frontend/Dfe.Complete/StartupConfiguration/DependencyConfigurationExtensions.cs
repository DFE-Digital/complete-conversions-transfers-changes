using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Infrastructure.Gateways;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Services.Project;
using Dfe.Complete.Validators;
using FluentValidation;

namespace Dfe.Complete.StartupConfiguration
{
    public static class DependencyConfigurationExtensions
    {
        public static IServiceCollection AddClientDependencies(this IServiceCollection services)
        {
            services.AddScoped<IErrorService, ErrorService>();
            services.AddScoped<CompleteApiClient, CompleteApiClient>();
            services.AddScoped<IAnalyticsConsentService, AnalyticsConsentService>();
            services.AddScoped<IProjectPermissionService, ProjectPermissionService>();
            services.AddScoped<IProjectService, ProjectService>();

            services.AddScoped<ITrustCache, TrustCacheService>();

            services.AddScoped<IValidator<ExternalContactInputModel>, ExternalContactInputValidator<ExternalContactInputModel>>();
            services.AddScoped<IValidator<OtherExternalContactInputModel>, OtherExternalContactInputValidator>();

            return services;
        }
    }
}
