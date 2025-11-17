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
        ErrorMessage = error switch
        {
            "duplicate_account" => "Your email address doesn't match the email associated with your account. This may indicate a duplicate account. Please contact service support.",
            "user_not_found" => "You do not have access to this system. Please contact your administrator if you believe this is an error.",
            "validation_failed" => "An error occurred during sign in. Please try again or contact support.",
            "no_principal" => "Authentication failed. Please try again.",
            _ => null
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = "/")
    {
        // If user is already authenticated, just return the page
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return Page();
        }

        // User is not authenticated, trigger the Azure AD / Entra challenge
        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl) || returnUrl == "/")
        {
            returnUrl = "/";
        }

        var props = new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };

        // Trigger the Azure AD / Entra challenge
        // Note: User validation will be handled in middleware after successful authentication
        return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
