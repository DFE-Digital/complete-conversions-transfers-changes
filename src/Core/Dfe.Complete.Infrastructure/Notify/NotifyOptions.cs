using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Infrastructure.Notify
{
    /// <summary>
    /// Configuration options for GOV.UK Notify email service.
    /// </summary>
    public class NotifyOptions
    {
        public const string Section = "Notify";

        [Required(ErrorMessage = "Notify API key is required")]
        public string ApiKey { get; set; } = string.Empty;

        public string? ServiceId { get; set; }

        public bool TestMode { get; set; } = false;

        public List<string> TestModeAllowedDomains { get; set; } = new() { "@education.gov.uk" };

        [Required]
        public EmailOptions Email { get; set; } = new();

        public RetryOptions Retry { get; set; } = new();

        public class EmailOptions
        {
            [Required]
            public Dictionary<string, string> Templates { get; set; } = new();

            public string? DefaultReplyToId { get; set; }

            [Required(ErrorMessage = "ProjectBaseUrl is required")]
            public string ProjectBaseUrl { get; set; } = "https://complete.education.gov.uk/projects/";
        }

        public class RetryOptions
        {
            [Range(0, 10, ErrorMessage = "MaxRetryAttempts must be between 0 and 10")]
            public int MaxRetryAttempts { get; set; } = 3;

            [Range(1, 60, ErrorMessage = "BaseDelaySeconds must be between 1 and 60")]
            public int BaseDelaySeconds { get; set; } = 2;
        }
    }
}

