using System.Net.Http.Headers;
using System.Security.Claims;
using AutoFixture;
using Dfe.AcademiesApi.Client;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Api.Client.Extensions;
using Dfe.Complete.Api.Tests.Integration.Factories;
using Dfe.Complete.Application.Common.Mappers;
using Dfe.Complete.Client;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Seeders;
using DfE.CoreLibs.Testing.Mocks.Authentication;
using Dfe.TramsDataApi.Client.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace Dfe.Complete.Api.Tests.Integration.Customizations
{
    public class CustomWebApplicationDbApiContextFactoryCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CustomWebApplicationDbApiContextFactory<Program>>(composer => composer.FromFactory(() =>
            {
                var factory = new CustomWebApplicationDbApiContextFactory<Program>
                {
                    SeedData = new Dictionary<Type, Action<DbContext>>
                    {
                        { typeof(CompleteContext), context => CompleteContextSeeder.Seed((CompleteContext)context, fixture) }
                    },
                    ExternalServicesConfiguration = services =>
                    {
                        services.PostConfigure<AuthenticationOptions>(options =>
                        {
                            options.DefaultAuthenticateScheme = "TestScheme";
                            options.DefaultChallengeScheme = "TestScheme";
                        });

                        services.AddAuthentication("TestScheme")
                            .AddScheme<AuthenticationSchemeOptions, MockJwtBearerHandler>("TestScheme", options => { });

                        services.AddAutoMapper(cfg =>
                        {
                            cfg.AddProfile<AutoMapping>();
                        });
                    },
                    ExternalHttpClientConfiguration = client =>
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "external-mock-token");
                    }
                };

                var client = factory.CreateClient();
                var apiUrl = factory.MockApiServer.Url;

                var config = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "CompleteApiClient:BaseUrl", client.BaseAddress!.ToString() },
                        { "AcademiesApiClient:BaseUrl", apiUrl}
                    })
                    .Build();

                var services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(config);
                services.AddSingleton<HttpClient>(factory.WireMockHttpClient);

                services.AddCompleteApiClient<IProjectsClient, ProjectsClient>(config, client);
                services.AddCompleteApiClient<ICsvExportClient, CsvExportClient>(config, client);
                services.AddCompleteApiClient<IUsersClient, UsersClient>(config, client);

                services.AddAcademiesApiClient<IEstablishmentsV4Client, EstablishmentsV4Client>(config, factory.WireMockHttpClient);

                var serviceProvider = services.BuildServiceProvider();

                var runtimeConfig = serviceProvider.GetRequiredService<IConfiguration>();

                Console.WriteLine($"AcademiesApiClient BaseUrl: {runtimeConfig["AcademiesApiClient:BaseUrl"]}");


                fixture.Inject(factory);
                fixture.Inject(serviceProvider);
                fixture.Inject(client);
                fixture.Inject(factory.WireMockHttpClient);
                fixture.Inject(serviceProvider.GetRequiredService<IProjectsClient>());
                fixture.Inject(serviceProvider.GetRequiredService<ICsvExportClient>());
                fixture.Inject(serviceProvider.GetRequiredService<IUsersClient>());
                fixture.Inject(serviceProvider.GetRequiredService<IEstablishmentsV4Client>());
                fixture.Inject(new List<Claim>());

                return factory;
            }));
        }
    }
}
