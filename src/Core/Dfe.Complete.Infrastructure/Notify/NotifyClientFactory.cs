using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Interfaces;

namespace Dfe.Complete.Infrastructure.Notify
{
    /// <summary>
    /// Factory for creating GOV.UK Notify client instances.
    /// Encapsulates client creation logic and API key configuration.
    /// </summary>
    public class NotifyClientFactory
    {
        private readonly NotifyOptions _options;

        public NotifyClientFactory(IOptions<NotifyOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Creates a configured NotificationClient instance.
        /// </summary>
        public IAsyncNotificationClient CreateClient()
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new InvalidOperationException("Notify API key is not configured");

            return new NotificationClient(_options.ApiKey);
        }
    }
}

