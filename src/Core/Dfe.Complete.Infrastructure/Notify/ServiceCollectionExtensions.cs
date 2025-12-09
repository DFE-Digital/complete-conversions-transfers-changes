using Dfe.Complete.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notify.Interfaces;
using System;

namespace Dfe.Complete.Infrastructure.Notify
{
    /// <summary>
    /// Dependency injection registration for GOV.UK Notify email services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds GOV.UK Notify email services to the DI container.
        /// Registers IEmailSender, ITemplateIdProvider, and NotifyOptions with validation.
        /// </summary>
        public static IServiceCollection AddNotifyEmailServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register and validate NotifyOptions
            var optionsBuilder = services.AddOptions<NotifyOptions>()
                .Bind(configuration.GetSection(NotifyOptions.Section))
                .ValidateDataAnnotations();

            // Only validate on startup if ApiKey is configured (not empty)
            // This allows NSwag to run during build without User Secrets
            // When the app runs normally, User Secrets will provide the ApiKey and validation will run
            var apiKey = configuration[$"{NotifyOptions.Section}:ApiKey"];
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                optionsBuilder.ValidateOnStart();
            }

            // Register template provider as singleton (configuration doesn't change)
            services.AddSingleton<ITemplateIdProvider, AppTemplateIdProvider>();

            // Register project URL builder as singleton (configuration doesn't change)
            services.AddSingleton<IProjectUrlBuilder, ProjectUrlBuilder>();

            // Register Notify client factory and client
            services.AddSingleton<NotifyClientFactory>();
            services.AddScoped<IAsyncNotificationClient>(sp =>
            {
                var factory = sp.GetRequiredService<NotifyClientFactory>();
                return factory.CreateClient();
            });

            // Register email sender as scoped (matches existing service patterns)
            services.AddScoped<IEmailSender, NotifyEmailSender>();

            return services;
        }
    }
}

