using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Public;

public class SignInModel : PageModel
{
    public string? ReturnUrl { get; private set; }
    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet(string? returnUrl = "/", string? error = null)
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/")
            returnUrl = null;

        ReturnUrl = returnUrl;

        // Set user-friendly error messages based on error parameter
        if (!string.IsNullOrEmpty(error))
        {
            var validationFailure = error.FromDescriptionValue<AuthenticationValidationFailure>();
            ErrorMessage = validationFailure switch
            {
                AuthenticationValidationFailure.DuplicateAccount => "Your email address doesn't match the email associated with your account. This may indicate a duplicate account. Please contact service support.",
                AuthenticationValidationFailure.UserNotFound => "You do not have access to this system.  Please contact service support if you believe this is an error.",
                AuthenticationValidationFailure.EmailConflict => "This email address may be registered to a different user account. Please contact service support.",
                AuthenticationValidationFailure.ValidationFailed => "An error occurred during sign in. Please try again or contact service support.",
                AuthenticationValidationFailure.NoPrincipal => "Authentication failed. Please try again or contact service support.",
                AuthenticationValidationFailure.NoEmail => "No email address found in your authentication details. Please try again or contact service support.",
                _ => null
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = "/")
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl) || returnUrl == "/")
            returnUrl = "/";

        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(returnUrl);

        var props = new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };

        // Trigger the Azure AD / Entra challenge
        // Note: User validation will be handled in middleware after successful authentication
        return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
