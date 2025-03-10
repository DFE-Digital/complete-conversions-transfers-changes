using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces.CsvExport;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Infrastructure.QueryServices;
using Dfe.Complete.Infrastructure.QueryServices.CsvExport;
using Dfe.Complete.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dfe.Complete.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureDependencyGroup(this IServiceCollection services, IConfiguration config)
        {
            //Repos
            services.AddScoped(typeof(ICompleteRepository<>), typeof(CompleteRepository<>));

            //Cache service
            services.AddServiceCaching(config);

            //Db
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<CompleteContext>(options => options.UseSqlServer(connectionString));
            
            //Queries
            services.AddScoped<IListAllProjectsQueryService, ListAllProjectsQueryService>();
            services.AddScoped<IConversionCsvQueryService, ConversionCsvQueryService>();
            services.AddScoped<IListAllProjectsForLocalAuthorityQueryService, ListAllProjectsForLocalAuthorityQueryService>();
            services.AddScoped<IListAllProjectsForRegionQueryService, ListAllProjectsForRegionQueryService>();
            
            // Authentication
            //services.AddCustomAuthorization(config);

            return services;
        }
    }
}