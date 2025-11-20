using Dfe.Complete.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Complete.Infrastructure.Notify
{
    /// <summary>
    /// Template ID provider that reads from NotifyOptions configuration.
    /// Maps logical template keys to GOV.UK Notify template IDs (GUIDs).
    /// </summary>
    public class AppTemplateIdProvider : ITemplateIdProvider
    {
        private readonly NotifyOptions _options;

        public AppTemplateIdProvider(IOptions<NotifyOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public string GetTemplateId(string templateKey)
        {
            if (string.IsNullOrWhiteSpace(templateKey))
                throw new ArgumentException("Template key cannot be empty", nameof(templateKey));

            if (!_options.Email.Templates.TryGetValue(templateKey, out var templateId))
                throw new KeyNotFoundException($"Template not found for key: {templateKey}");

            return templateId;
        }

        public bool TemplateExists(string templateKey)
        {
            if (string.IsNullOrWhiteSpace(templateKey))
                return false;

            return _options.Email.Templates.ContainsKey(templateKey);
        }
    }
}

