using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Dfe.Complete.Pages.Errors
{
	public class IndexModel(ILogger<IndexModel> logger) : PageModel
	{
        public string RequestId { get; private set; } = string.Empty;

        public string ErrorMessage { get; private set; } = "Sorry, there is a problem with the service";

		public void OnGet(int? statusCode = null)
        {
            ManageErrors(statusCode);
		}

		public void OnPost(int? statusCode = null)
        { 
            ManageErrors(statusCode);
		}

		private void ManageErrors(int? statusCode)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            if (!statusCode.HasValue)
			{
				ManageUnhandledErrors();
				return;
			}

			ErrorMessage = statusCode.Value switch
			{
				404 => "Page not found",
				500 => "Internal server error",
				501 => "Not implemented",
				_ => $"Error {statusCode}"
			};
            logger.LogInformation("ErrorPage::Something went wrong - {RequestId}, {ErrorMessage}", RequestId, ErrorMessage);
        }

        private void ManageUnhandledErrors()
        {
            var unhandledError = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
            logger.LogInformation("ErrorPage::Something went wrong - {RequestId}, {Message}", RequestId, unhandledError?.Message);

            // Thrown by RedirectToPage when the name of the page is incorrect.  
            if (unhandledError is InvalidOperationException && unhandledError.Message.Contains("no page named", StringComparison.CurrentCultureIgnoreCase))
            {
                ErrorMessage = "Page not found";
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            }
        }
   	}
}