using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Public
{
	public class Cookies : PageModel
	{
		public bool? Consent { get; set; }
		public bool PreferencesSet { get; set; } = false;
		public string returnPath { get; set; }

		private readonly IAnalyticsConsentService _analyticsConsentService;

		public Cookies(ILogger<Cookies> logger, IAnalyticsConsentService analyticsConsentService)
		{
			_analyticsConsentService = analyticsConsentService;
		}

		public string TransfersCookiesUrl { get; set; }
		private ActionResult ProcessConsent(bool? consent, string returnUrl, bool noRedirect = false)
		{
			returnPath = returnUrl;
			Consent = _analyticsConsentService.ConsentValue();

			if (consent.HasValue)
			{
				PreferencesSet = true;
				TempData["PreferencesSet"] = true;

				var valueToApply = consent ?? Consent ?? false;
				ApplyCookieConsent(valueToApply);

				if (!string.IsNullOrEmpty(returnUrl) && noRedirect)
				{
					return Redirect(returnUrl);
				}
				return RedirectToPage(Links.Public.CookiePreferences);
			}
			return Page();
		}

		public ActionResult OnGet(bool? consent, string returnUrl)
		{
			if (TempData["PreferencesSet"] is bool prefsSet && prefsSet)
			{
				PreferencesSet = true;
			}
			return ProcessConsent(consent, returnUrl);
		}

		public ActionResult OnPost(bool? consent, string returnUrl, bool noRedirect = false)
		{
			returnPath = returnUrl;
			return ProcessConsent(consent, returnUrl, noRedirect);
		}

		private void ApplyCookieConsent(bool consent)
		{
			if (consent)
			{
				_analyticsConsentService.AllowConsent();
			}
			else
			{
				_analyticsConsentService.DenyConsent();
			}
		}
	}
}