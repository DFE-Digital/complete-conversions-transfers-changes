using Dfe.AcademiesApi.Client.Contracts;
using Dfe.AcademiesApi.Client;
using Dfe.Complete.Application.Common.Behaviours;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Application.Services.CsvExport.Conversion;
using Dfe.TramsDataApi.Client.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Dfe.Complete.Application.Services.TrustCache;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationDependencyGroup(
            this IServiceCollection services, IConfiguration config)
        {
            var performanceLoggingEnabled = config.GetValue<bool>("Features:PerformanceLoggingEnabled");

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddAcademiesApiClient<ITrustsV4Client, TrustsV4Client>(config);
            services.AddAcademiesApiClient<IEstablishmentsV4Client, EstablishmentsV4Client>(config);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
                
                if (performanceLoggingEnabled)
                {
                    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
                }
            });

            services.AddScoped<ICSVFileContentGenerator<ConversionCsvModel>, CSVFileContentGenerator<ConversionCsvModel>>();
            services.AddScoped<ITrustCache, TrustCacheService>();
            services.AddScoped<IRowBuilderFactory<ConversionCsvModel>, RowBuilderFactory<ConversionCsvModel>>();
            services.AddScoped<IRowGenerator<ConversionCsvModel>, ConversionRowGenerator>();
            services.AddScoped<IHeaderGenerator<ConversionCsvModel>, ConversionRowGenerator>();

            services.AddBackgroundService();

            return services;
        }
    }
}
