using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Public;

[AllowAnonymous]
public class SignInModel : PageModel
{
    public string? ReturnUrl { get; private set; }

    public IActionResult OnGet(string? returnUrl = "/")
    {
        // Normalise return URL
        if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/")
        {
            returnUrl = null;
        }

        // Already signed in? Just bounce them back.
        if (User?.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect(returnUrl ?? "/");
        }

        ReturnUrl = returnUrl;
        return Page();
    }

    public IActionResult OnPost(string? returnUrl = "/")
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl) || returnUrl == "/")
        {
            returnUrl = "/";
        }

        var props = new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };

        // Trigger the Azure AD / Entra challenge
        return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
