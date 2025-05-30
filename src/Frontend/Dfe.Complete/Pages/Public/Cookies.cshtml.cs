using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Public
{
	[AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class Cookies(ILogger<Cookies> logger, IAnalyticsConsentService analyticsConsentService) : PageModel
	{
		public bool? Consent { get; set; }
		public bool PreferencesSet { get; set; } = false;
		public string ReturnPath { get; set; } = string.Empty;

        public string TransfersCookiesUrl { get; set; } = string.Empty;

        public ActionResult OnGet(bool? consent, string returnUrl)
		{
            ReturnPath = returnUrl;

			Consent = analyticsConsentService.ConsentValue();

            if (consent.HasValue)
			{
				PreferencesSet = true;

				ApplyCookieConsent(consent.Value);

				if (!string.IsNullOrEmpty(returnUrl))
				{
					return Redirect(returnUrl);
				}

				return RedirectToPage(Links.Public.CookiePreferences);
			}

			return Page();
		}

        public IActionResult OnPost([FromBody] Dictionary<string, string> formData, [FromQuery] string? returnUrl)
        {
            if (formData.TryGetValue("cookies_form[accept_optional_cookies]", out var acceptOptionalCookiesValue))
            {
                bool? acceptOptionalCookies = bool.TryParse(acceptOptionalCookiesValue, out var parsedValue) ? parsedValue : (bool?)null;

				logger.LogInformation("Testing returnUrl - {returnUrl} cookies_form[accept_optional_cookies] - {@AcceptOptionalCookies}", returnUrl, acceptOptionalCookies);
				ReturnPath = formData.TryGetValue("returnUrl", out string? value) ? value : Links.Public.CookiePreferences.Page; 

                Consent = analyticsConsentService.ConsentValue();

                if (acceptOptionalCookies.HasValue)
                {
                    Consent = acceptOptionalCookies;
                    PreferencesSet = true;

                    ApplyCookieConsent(acceptOptionalCookies.Value);
                    return Page();
                }
            }

            return Page();
        }
		public IActionResult OnPost(bool? consent, string returnUrl)
		{
			if (returnUrl == null)
			{
				returnUrl = Links.Public.CookiePreferences.Page;

			}
			ReturnPath = returnUrl;

			Consent = analyticsConsentService.ConsentValue();

			if (consent.HasValue)
			{
				Consent = consent;
				PreferencesSet = true;

				ApplyCookieConsent(consent.Value);
				return Page();
			}

			return Page();
		}

		private void ApplyCookieConsent(bool consent)
		{
			if (consent) { 
				analyticsConsentService.AllowConsent();
			}
			else
			{
				analyticsConsentService.DenyConsent();
			}
		}
	}
}