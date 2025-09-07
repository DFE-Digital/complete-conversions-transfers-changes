using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.ExternalContacts
{
    [Authorize(Policy = UserPolicyConstants.CanEditDeleteContact)]
    public class DeleteExternalContact(ISender sender, ErrorService errorService, ILogger<DeleteExternalContact> logger)
     : PageModel
    {   
        private readonly ISender sender = sender;
        private readonly ErrorService errorService = errorService;
        private readonly ILogger<DeleteExternalContact> logger = logger;

        [BindProperty(SupportsGet = true, Name = "projectId")]
        public string ProjectId { get; set; }

        [BindProperty(SupportsGet = true, Name = "contactId")]
        public string ContactId { get; set; }

        public string FullName { get; set; }

        public string Role { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return await this.GetPage();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try{

                var deleteExternalContactCommand = new DeleteExternalContactCommand(new ContactId(Guid.Parse(ContactId)));
                var response = await sender.Send(deleteExternalContactCommand);

                if (!(response.IsSuccess || response.Value == true))
                {
                    throw new InvalidOperationException($"An error occurred when deleting contact {ContactId} for project {ProjectId}");
                }

                TempData.SetNotification(
                    NotificationType.Success,
                    "Success",
                    "Contact deleted"
                );

                return Redirect(string.Format(RouteConstants.ProjectExternalContacts, ProjectId));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred on deleting an external contact for project {ProjectId}", ProjectId);
                ModelState.AddModelError("UnexpectedError", "An unexpected error occurred. Please try again later.");
                errorService.AddErrors(ModelState);
                return await this.GetPage();
            }
        }

        private async Task<IActionResult> GetPage()
        {   
            await this.GetContactDetails();
            return Page();
        }

        private async Task GetContactDetails()
        {
            var success = Guid.TryParse(ContactId, out var guid);

            if (!success)
            {
                var error = $"{ContactId} is not a valid Guid.";
                logger.LogError(error);
                return;
            }

            var query = new GetContactByIdQuery(new ContactId(guid));
            var result = await sender.Send(query);

            if (!result.IsSuccess || result.Value == null)
            {
                var error = $"Contact {ContactId} does not exist.";
                logger.LogError(error);
                return;
            }

            var contact = result.Value;

            FullName = contact.Name;
            Role = contact.Title;
        }
    }

}
