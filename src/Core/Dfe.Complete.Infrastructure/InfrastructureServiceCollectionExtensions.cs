using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Infrastructure.Repositories;
using Dfe.Complete.Infrastructure.Security.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureDependencyGroup(
            this IServiceCollection services, IConfiguration config)
        {
            //Repos
            services.AddScoped<ISchoolRepository, SchoolRepository>();
            services.AddScoped(typeof(ISclRepository<>), typeof(SclRepository<>));
            services.AddScoped(typeof(ICompleteRepository<>), typeof(CompleteRepository<>));

            //Cache service
            services.AddServiceCaching(config);

            //Db
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<SclContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<CompleteContext>(options => options.UseSqlServer(connectionString));

            // Authentication
            //services.AddCustomAuthorization(config);

            return services;
        }
    }
}