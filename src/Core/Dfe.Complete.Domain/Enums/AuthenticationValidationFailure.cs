using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums;

/// <summary>
/// Represents the different types of authentication validation failures
/// </summary>
public enum AuthenticationValidationFailure
{
    [Description("no_principal")]
    NoPrincipal,
    [Description("no_email")]
    NoEmail,
    [Description("validation_failed")]
    ValidationFailed,
    [Description("duplicate_account")]
    DuplicateAccount,
    [Description("user_not_found")]
    UserNotFound,
    [Description("email_conflict")]
    EmailConflict
}