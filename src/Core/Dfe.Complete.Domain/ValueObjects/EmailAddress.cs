using System.Text.RegularExpressions;

namespace Dfe.Complete.Domain.ValueObjects
{
    /// <summary>
    /// Represents a validated email address value object.
    /// </summary>
    public partial record EmailAddress(string Value)
    {
        private static readonly Regex EmailRegex = GenerateEmailRegex();

        /// <summary>
        /// Gets the domain part of the email address (e.g., "education.gov.uk" from "user@education.gov.uk")
        /// </summary>
        public string Domain => Value.Contains('@') ? Value.Split('@')[1] : string.Empty;

        /// <summary>
        /// Creates an EmailAddress with validation.
        /// </summary>
        /// <param name="email">The email address string</param>
        /// <returns>EmailAddress instance</returns>
        /// <exception cref="ArgumentException">Thrown when email is invalid</exception>
        public static EmailAddress Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email address cannot be empty", nameof(email));

            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException($"Invalid email address format: {email}", nameof(email));

            return new EmailAddress(email.Trim().ToLowerInvariant());
        }

        /// <summary>
        /// Implicit conversion from string to EmailAddress.
        /// Throws ArgumentException if email is invalid.
        /// </summary>
        public static implicit operator EmailAddress?(string? email)
        {
            return string.IsNullOrWhiteSpace(email) ? null : Create(email);
        }

        /// <summary>
        /// Implicit conversion from EmailAddress to string.
        /// </summary>
        public static implicit operator string(EmailAddress emailAddress)
        {
            return emailAddress.Value;
        }

        public override string ToString() => Value;

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
        private static partial Regex GenerateEmailRegex();
    }
}

