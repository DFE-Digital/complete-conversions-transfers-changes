using Dfe.Complete.Application.Common.Behaviours;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport;
using Dfe.Complete.Application.Services.CsvExport.Conversion;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationDependencyGroup(
            this IServiceCollection services, IConfiguration config)
        {
            var performanceLoggingEnabled = config.GetValue<bool>("Features:PerformanceLoggingEnabled");

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

                if (performanceLoggingEnabled)
                {
                    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
                }
            });

            services.AddScoped<ICSVFileContentGenerator<ConversionCsvModel>, CSVFileContentGenerator<ConversionCsvModel>>();
            services.AddScoped<IRowGenerator<ConversionCsvModel>, ConversionRowGenerator>();
            services.AddScoped<IHeaderGenerator<ConversionCsvModel>, ConversionRowGenerator>();

            services.AddBackgroundService();

            return services;
        }
    }
}
