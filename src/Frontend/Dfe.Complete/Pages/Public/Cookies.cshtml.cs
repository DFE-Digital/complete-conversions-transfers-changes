using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Public
{
	[AllowAnonymous] 
    public class Cookies(IAnalyticsConsentService analyticsConsentService) : PageModel
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

        public IActionResult OnPost(bool? consent, string returnUrl, [FromForm(Name ="cookies_form[accept_optional_cookies]")] bool? cookiesConsent)
		{
			ReturnPath = returnUrl;
			 

            if (!consent.HasValue)
            {
				consent = cookiesConsent;
            }

            Consent = analyticsConsentService.ConsentValue();

			if (consent.HasValue)
			{
				Consent = consent;
				PreferencesSet = true;

                ApplyCookieConsent(consent.Value);

                if (cookiesConsent.HasValue)
                {
					//var returnURl = Request.Headers["Referer"].ToString().Replace("https://", string.Empty).Replace(HttpContext.Request.Host.Value, string.Empty);

                    return RedirectToPage(Request.Headers["Referer"].ToString());
                }
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