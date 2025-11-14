using Dfe.Complete.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Complete.Infrastructure.Notify
{
    /// <summary>
    /// Builds project URLs using the configured base URL from Notify options.
    /// </summary>
    public class ProjectUrlBuilder : IProjectUrlBuilder
    {
        private readonly string _projectBaseUrl;

        public ProjectUrlBuilder(IOptions<NotifyOptions> options)
        {
            _projectBaseUrl = options.Value.Email.ProjectBaseUrl;
        }

        public string BuildProjectUrl(string projectReference)
        {
            return $"{_projectBaseUrl}{projectReference}";
        }
    }
}

