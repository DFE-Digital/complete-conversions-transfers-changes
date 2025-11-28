using AutoFixture;
using Dfe.AcademiesApi.Client;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.AcademiesApi.Client.Security;
using Dfe.AcademiesApi.Client.Settings;
using Dfe.Complete.Api.Client.Extensions;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Mappers;
using Dfe.Complete.Client;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Seeders;
using GovUK.Dfe.CoreLibs.Testing.Mocks.Authentication;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using GovUK.Dfe.PersonsApi.Client;
using GovUK.Dfe.PersonsApi.Client.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Customizations
{
    public class CustomWebApplicationDbContextFactoryCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CustomWebApplicationDbContextFactory<Program>>(composer => composer.FromFactory(() =>
            {
                var factory = new CustomWebApplicationDbContextFactory<Program>()
                {
                    UseWireMock = true,
                    WireMockPort = 0,
                    SeedData = new Dictionary<Type, Action<DbContext>>
                    {
                        {
                            typeof(CompleteContext),
                            context => CompleteContextSeeder.Seed((CompleteContext)context, fixture)
                        }
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

                        services.AddAutoMapper(cfg => { cfg.AddProfile<AutoMapping>(); });

                        // Register mock email sender BEFORE infrastructure services
                        // This will be overridden by AddNotifyEmailServices, so we need to remove and re-add after
                        services.AddScoped<IEmailSender, NoOpEmailSender>();
                    },
                    ExternalHttpClientConfiguration = client =>
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", "external-mock-token");
                    },
                    ExternalWireMockConfigOverride = (cfgBuilder, mockServer) =>
                    {
                        cfgBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["AcademiesApiClient:BaseUrl"] = mockServer.Urls[0].TrimEnd('/') + "/",
                        });

                        cfgBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["PersonsApiClient:BaseUrl"] = mockServer.Urls[0].TrimEnd('/') + "/",
                        });

                        // Add minimal Notify configuration to prevent validation errors in tests
                        cfgBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["Notify:ApiKey"] = "test-api-key",
                            ["Notify:TestMode"] = "true",
                            ["Notify:Email:Templates:NewAccountAdded"] = "test-template-id",
                            ["Notify:Email:Templates:NewConversionProjectCreated"] = "test-template-id",
                            ["Notify:Email:Templates:NewTransferProjectCreated"] = "test-template-id",
                            ["Notify:Email:Templates:AssignedNotificationConversion"] = "test-template-id",
                            ["Notify:Email:Templates:AssignedNotificationTransfer"] = "test-template-id",
                            ["Notify:Email:ProjectBaseUrl"] = "https://test.com/projects/",
                            ["FeatureManagement:EmailNotifications"] = "true"
                        });
                    },
                    ExternalWireMockClientRegistration = (services, config, wireHttp) =>
                    {
                        services.AddHttpClient<IEstablishmentsV4Client, EstablishmentsV4Client>(
                                (httpClient, serviceProvider) =>
                                {
                                    var wConfig = serviceProvider.GetRequiredService<IConfiguration>();

                                    httpClient.BaseAddress = new Uri(wConfig["AcademiesApiClient:BaseUrl"]!);

                                    return ActivatorUtilities.CreateInstance<EstablishmentsV4Client>(
                                        serviceProvider, httpClient, wConfig["AcademiesApiClient:BaseUrl"]!);
                                })
                            .AddHttpMessageHandler(serviceProvider =>
                            {
                                var apiSettings = serviceProvider.GetRequiredService<AcademiesApiClientSettings>();
                                return new ApiKeyHandler(apiSettings);
                            });

                        services.AddHttpClient<ITrustsV4Client, TrustsV4Client>(
                                (httpClient, serviceProvider) =>
                                {
                                    var wConfig = serviceProvider.GetRequiredService<IConfiguration>();

                                    httpClient.BaseAddress = new Uri(wConfig["AcademiesApiClient:BaseUrl"]!);

                                    return ActivatorUtilities.CreateInstance<TrustsV4Client>(
                                        serviceProvider, httpClient, wConfig["AcademiesApiClient:BaseUrl"]!);
                                })
                            .AddHttpMessageHandler(serviceProvider =>
                            {
                                var apiSettings = serviceProvider.GetRequiredService<AcademiesApiClientSettings>();
                                return new ApiKeyHandler(apiSettings);
                            });

                        services.AddHttpClient<IConstituenciesClient, ConstituenciesClient>(
                               (httpClient, serviceProvider) =>
                               {
                                   var wConfig = serviceProvider.GetRequiredService<IConfiguration>();

                                   httpClient.BaseAddress = new Uri(wConfig["PersonsApiClient:BaseUrl"]!);

                                   return ActivatorUtilities.CreateInstance<ConstituenciesClient>(
                                       serviceProvider, httpClient, wConfig["PersonsApiClient:BaseUrl"]!);
                               });
                    }
                };

                var client = factory.CreateClient();

                var config = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "CompleteApiClient:BaseUrl", client.BaseAddress!.ToString() }
                    })
                    .Build();

                var services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(config);

                services.AddCompleteApiClient<IProjectsClient, ProjectsClient>(config, client);
                services.AddCompleteApiClient<IProjectGroupClient, ProjectGroupClient>(config, client);
                services.AddCompleteApiClient<ITasksDataClient, TasksDataClient>(config, client);
                services.AddCompleteApiClient<ICsvExportClient, CsvExportClient>(config, client);
                services.AddCompleteApiClient<IUsersClient, UsersClient>(config, client);
                services.AddCompleteApiClient<IServiceSupportClient, ServiceSupportClient>(config, client);
                services.AddCompleteApiClient<IContactsClient, ContactsClient>(config, client);
                var serviceProvider = services.BuildServiceProvider();

                fixture.Inject(factory);
                fixture.Inject(serviceProvider);
                fixture.Inject(client);
                fixture.Inject(serviceProvider.GetRequiredService<IProjectsClient>());
                fixture.Inject(serviceProvider.GetRequiredService<IProjectGroupClient>());
                fixture.Inject(serviceProvider.GetRequiredService<ITasksDataClient>());
                fixture.Inject(serviceProvider.GetRequiredService<ICsvExportClient>());
                fixture.Inject(serviceProvider.GetRequiredService<IUsersClient>());
                fixture.Inject(serviceProvider.GetRequiredService<IServiceSupportClient>());
                fixture.Inject(serviceProvider.GetRequiredService<IContactsClient>());
                fixture.Inject(new List<Claim>());

                return factory;
            }));
        }
    }

    /// <summary>
    /// No-op email sender implementation for integration tests.
    /// Prevents email sending failures from breaking tests.
    /// </summary>
    internal class NoOpEmailSender : IEmailSender
    {
        public Task<Result<EmailSendResult>> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            // Return success without actually sending emails in tests
            return Task.FromResult(Result<EmailSendResult>.Success(
                new EmailSendResult(
                    ProviderMessageId: "test-message-id",
                    Reference: message.Reference ?? "test-reference",
                    SentAt: DateTime.UtcNow)));
        }
    }
}