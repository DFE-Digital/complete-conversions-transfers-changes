using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Exceptions;
using Notify.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Infrastructure.Notify
{
    /// <summary>
    /// GOV.UK Notify implementation of IEmailSender with manual retry logic.
    /// Sends emails through GOV.UK Notify API with automatic retries on transient failures.
    /// </summary>
    public class NotifyEmailSender : IEmailSender
    {
        private readonly IAsyncNotificationClient _notifyClient;
        private readonly ITemplateIdProvider _templateProvider;
        private readonly ILogger<NotifyEmailSender> _logger;
        private readonly NotifyOptions _options;

        public NotifyEmailSender(
            IAsyncNotificationClient notifyClient,
            ITemplateIdProvider templateProvider,
            ILogger<NotifyEmailSender> logger,
            IOptions<NotifyOptions> options)
        {
            ArgumentNullException.ThrowIfNull(notifyClient);
            ArgumentNullException.ThrowIfNull(templateProvider);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(options);
            
            _notifyClient = notifyClient;
            _templateProvider = templateProvider;
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// Sends an email asynchronously with automatic retry on transient failures.
        /// </summary>
        public async Task<Result<EmailSendResult>> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);

            // Validate test mode first
            if (_options.TestMode && !IsAllowedTestDomain(message.To.Value))
            {
                // Log domain only (not full email) to avoid PII exposure
                var domain = ExtractDomain(message.To.Value);
                _logger.LogWarning(
                    "Email blocked in test mode. Template: {TemplateKey}, Domain: {Domain}",
                    message.TemplateKey, domain);
                
                return Result<EmailSendResult>.Failure(
                    "Email blocked in test mode",
                    ErrorType.UnprocessableContent);
            }

            // Generate reference for idempotency if not provided
            var reference = message.Reference ?? GenerateReference(message);

            // Get template ID
            try
            {
                var templateId = _templateProvider.GetTemplateId(message.TemplateKey);

                // Call retry logic
                return await SendWithRetryAsync(
                    message.To.Value,
                    templateId,
                    message.Personalisation,
                    reference,
                    message.ReplyToId,
                    message.TemplateKey,
                    cancellationToken);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Template not found: {TemplateKey}", message.TemplateKey);
                // Don't expose template key in user-facing error message for security
                return Result<EmailSendResult>.Failure(
                    "Email template not found",
                    ErrorType.NotFound);
            }
        }

        /// <summary>
        /// Sends email with manual retry loop (no external library).
        /// Implements exponential backoff with configurable retry attempts.
        /// </summary>
        private async Task<Result<EmailSendResult>> SendWithRetryAsync(
            string emailAddress,
            string templateId,
            IReadOnlyDictionary<string, string> personalisation,
            string reference,
            string? replyToId,
            string templateKey,
            CancellationToken cancellationToken)
        {
            var maxRetries = _options.Retry.MaxRetryAttempts;
            Exception? lastException = null;

            // MANUAL RETRY LOOP - Simple for loop, no Polly
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Convert personalisation to Dictionary<string, dynamic> for Notify API
                    var personalisationDict = personalisation?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => (dynamic)kvp.Value) ?? new Dictionary<string, dynamic>();

                    // Send email via Notify API
                    var response = await _notifyClient.SendEmailAsync(
                        emailAddress,
                        templateId,
                        personalisationDict,
                        reference,
                        replyToId);

                    // SUCCESS - log and return immediately
                    _logger.LogInformation(
                        "Email sent successfully. Template: {TemplateKey}, Reference: {Reference}, MessageId: {MessageId}, Attempt: {Attempt}",
                        templateKey, reference, response.id, attempt + 1);

                    return Result<EmailSendResult>.Success(new EmailSendResult(
                        ProviderMessageId: response.id,
                        Reference: reference,
                        SentAt: DateTime.UtcNow));
                }
                catch (Exception ex) when (IsTransientFailure(ex) && attempt < maxRetries)
                {
                    // TRANSIENT FAILURE - wait and retry
                    lastException = ex;
                    var delay = CalculateDelay(attempt, _options.Retry.BaseDelaySeconds);

                    _logger.LogWarning(
                        ex,
                        "Email send failed (transient). Attempt: {Attempt}/{MaxAttempts}, Retrying in {Delay}ms. Template: {TemplateKey}, Reference: {Reference}",
                        attempt + 1, maxRetries + 1, delay.TotalMilliseconds, templateKey, reference);

                    // Wait before retrying (exponential backoff)
                    await Task.Delay(delay, cancellationToken);

                    // Loop continues to next attempt
                }
                catch (Exception ex)
                {
                    // PERMANENT FAILURE - don't retry, return error immediately
                    _logger.LogError(
                        ex,
                        "Email send failed (permanent). Attempt: {Attempt}, Template: {TemplateKey}, Reference: {Reference}",
                        attempt + 1, templateKey, reference);

                    // Don't expose internal exception details to callers
                    return Result<EmailSendResult>.Failure(
                        "Email send failed due to a permanent error",
                        MapToErrorType(ex));
                }
            }

            // ALL RETRIES EXHAUSTED - return failure
            var totalAttempts = maxRetries + 1;
            if (lastException != null)
            {
                _logger.LogError(
                    lastException,
                    "Email send failed after {MaxAttempts} attempts. Template: {TemplateKey}, Reference: {Reference}",
                    totalAttempts, templateKey, reference);
            }
            else
            {
                _logger.LogError(
                    "Email send failed after {MaxAttempts} attempts. Template: {TemplateKey}, Reference: {Reference}",
                    totalAttempts, templateKey, reference);
            }

            // Don't expose internal exception details to callers
            var errorMessage = $"Email send failed after {totalAttempts} attempts";
            
            return Result<EmailSendResult>.Failure(errorMessage, ErrorType.Unknown);
        }

        /// <summary>
        /// Determines if an exception represents a transient failure that should be retried.
        /// </summary>
        private static bool IsTransientFailure(Exception ex)
        {
            // Network errors - retry
            if (ex is HttpRequestException)
                return true;

            // Timeouts - retry
            if (ex is TaskCanceledException)
                return true;

            // Notify API 5xx errors - retry
            if (ex is NotifyClientException notifyEx)
            {
                var statusCode = GetStatusCodeFromNotifyException(notifyEx);
                if (statusCode >= 500 && statusCode < 600)
                    return true;
            }

            // All other exceptions - don't retry (permanent failures)
            return false;
        }

        /// <summary>
        /// Extracts HTTP status code from Notify exception message.
        /// </summary>
        private static int GetStatusCodeFromNotifyException(NotifyClientException ex)
        {
            // Notify exceptions typically contain status code in the message
            // Example: "Status code: 500. Error response: ..."
            const string StatusCodePattern = @"Status code:\s*(\d+)";
            const int RegexTimeoutMilliseconds = 1000; // 1 second timeout to prevent ReDoS
            var statusMatch = Regex.Match(
                ex.Message, 
                StatusCodePattern, 
                RegexOptions.None, 
                TimeSpan.FromMilliseconds(RegexTimeoutMilliseconds));
            if (statusMatch.Success && int.TryParse(statusMatch.Groups[1].Value, out var code))
            {
                return code;
            }
            return 0;
        }

        /// <summary>
        /// Calculates exponential backoff delay for retry attempts.
        /// </summary>
        private static TimeSpan CalculateDelay(int attempt, int baseDelaySeconds)
        {
            // Exponential backoff: attempt 0 = 2s, attempt 1 = 4s, attempt 2 = 8s
            const int ExponentialBase = 2;
            var delaySeconds = baseDelaySeconds * Math.Pow(ExponentialBase, attempt);
            return TimeSpan.FromSeconds(delaySeconds);
        }

        /// <summary>
        /// Generates a deterministic reference for idempotency.
        /// Uses SHA256 hash to prevent duplicate emails during retries.
        /// Note: Email address is included in hash for idempotency - this is necessary and the hash is not logged.
        /// </summary>
        private static string GenerateReference(EmailMessage message)
        {
            // Hash of template key + email + personalisation keys for idempotency
            // Email is hashed (not logged) to ensure idempotency per recipient
            // SonarCloud: Email address is hashed for idempotency, not logged or exposed
            const int ReferenceLength = 32;
            var emailValue = message.To.Value; // Extract once to avoid repeated property access
            var personalisationKeys = string.Join(",", message.Personalisation.Keys.OrderBy(k => k));
            var content = string.Join("|", message.TemplateKey, emailValue, personalisationKeys);
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(content));
            return Convert.ToHexString(hash)[..ReferenceLength];
        }

        /// <summary>
        /// Maps exceptions to ErrorType enum.
        /// </summary>
        private static ErrorType MapToErrorType(Exception ex)
        {
            if (ex is NotifyClientException notifyEx)
            {
                var statusCode = GetStatusCodeFromNotifyException(notifyEx);
                if (statusCode == 404)
                    return ErrorType.NotFound;
                if (statusCode >= 400 && statusCode < 500)
                    return ErrorType.UnprocessableContent;
                if (statusCode >= 500)
                    return ErrorType.Unknown;
            }

            if (ex is HttpRequestException)
                return ErrorType.Unknown;

            if (ex is TaskCanceledException)
                return ErrorType.Unknown;

            return ErrorType.Unknown;
        }

        /// <summary>
        /// Checks if email domain is allowed in test mode.
        /// </summary>
        private bool IsAllowedTestDomain(string email)
        {
            var domain = ExtractDomain(email);
            return _options.TestModeAllowedDomains.Any(allowedDomain =>
                domain.EndsWith(allowedDomain.TrimStart('@'), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Extracts domain from email address.
        /// </summary>
        private static string ExtractDomain(string email)
        {
            const char AtSymbol = '@';
            var atIndex = email.IndexOf(AtSymbol, StringComparison.Ordinal);
            return atIndex >= 0 ? email.Substring(atIndex) : string.Empty;
        }
    }
}

