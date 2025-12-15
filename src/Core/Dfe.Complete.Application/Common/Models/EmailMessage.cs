using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Common.Models
{
    /// <summary>
    /// Represents an email message to be sent.
    /// DTO for email sending requests with template-based content.
    /// </summary>
    public record EmailMessage(
        EmailAddress To,
        string TemplateKey,
        IReadOnlyDictionary<string, string> Personalisation,
        string? Reference = null,
        string? ReplyToId = null,
        IReadOnlyList<string>? Tags = null)
    {
        /// <summary>
        /// Creates an EmailMessage with a dictionary for personalisation.
        /// </summary>
        public EmailMessage(
            EmailAddress to,
            string templateKey,
            Dictionary<string, string> personalisation,
            string? reference = null,
            string? replyToId = null,
            List<string>? tags = null)
            : this(to, templateKey, personalisation, reference, replyToId, tags?.AsReadOnly())
        {
        }
    }
}

