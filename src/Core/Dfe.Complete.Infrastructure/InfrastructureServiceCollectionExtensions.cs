using Dfe.Complete.Application.Common.Interfaces;
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
using StackExchange.Redis;

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

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Queries
            services.AddScoped<IListAllProjectsQueryService, ListAllProjectsQueryService>();
            services.AddScoped<IProjectsQueryBuilder, ProjectsQueryBuilder>();
            services.AddScoped<IConversionCsvQueryService, ConversionCsvQueryService>();

            // Authentication
            //services.AddCustomAuthorization(config);

            AddInfrastructureHealthChecks(services);

            var redisAppSettings = config.GetSection("Redis");
            if (redisAppSettings.GetValue<bool>("Enable"))
            {
                // Configure Redis Based Distributed Session
                var redisConfigurationOptions = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    ResolveDns = true,
                    Ssl = true,
                    EndPoints =
                    {
                        { redisAppSettings.GetValue<string>("Host")!, redisAppSettings.GetValue<int>("Port") }
                    },
                    Password = redisAppSettings.GetValue<string>("Password"),
                    ClientName = "Dfe.Complete",
                    DefaultVersion = new Version(6, 0),
                    AsyncTimeout = 15000,
                    SyncTimeout = 15000,
                };

                // https://stackexchange.github.io/StackExchange.Redis/ThreadTheft.html
                ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);

                IConnectionMultiplexer redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConfigurationOptions);

                services.AddStackExchangeRedisCache(redisCacheConfig =>
                {
                    redisCacheConfig.ConfigurationOptions = redisConfigurationOptions;
                    redisCacheConfig.ConnectionMultiplexerFactory = () => Task.FromResult(redisConnectionMultiplexer);
                    redisCacheConfig.InstanceName = "redis-master";
                });
            }

            return services;
        }

        public static void AddInfrastructureHealthChecks(this IServiceCollection services) {
            services.AddHealthChecks()
                .AddDbContextCheck<CompleteContext>("Complete Database");
        }
    }
}
