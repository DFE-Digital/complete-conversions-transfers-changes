using Dfe.Complete.Application.LocalAuthorities.Commands;
using Dfe.Complete.Application.LocalAuthorities.Queries;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.LocalAuthorities
{
    [Authorize(policy: UserPolicyConstants.CanViewServiceSupport)]
    public class DeleteLocalAuthorityModel(ISender sender) : ServiceSupportModel(LocalAuthoriesNavigation)
    {
        public required string Name { get; set; }

        [BindProperty(SupportsGet = true, Name = "id")]
        public required Guid Id { get; set; }

        [BindProperty(Name = nameof(ContactId))]
        public Guid? ContactId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var localAuthorityResponse = await sender.Send(new GetLocalAuthorityDetailsQuery(new LocalAuthorityId(Id)));
            var details = localAuthorityResponse?.Value!;
            Name = details.LocalAuthority.Name;
            ContactId = details.Contact?.Id.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var contactId = ContactId.HasValue ? new ContactId(ContactId.Value) : null;
            var response = await sender.Send(new DeleteLocalAuthorityCommand(new LocalAuthorityId(Id), contactId));
            if (response.IsSuccess)
            {
                TempData["HasDeletedLa"] = true;
                return RedirectToPage(Links.LocalAuthorities.ListLocalAuthorities);
            }
            return Page();
        }

        public string EditLocalAuthorityUrl(string id)
           => string.Format(RouteConstants.EditLocalAuthorityDetails, id);

        public string GetLocalAuthorityDetailsUrl(string id)
          => string.Format(RouteConstants.LocalAuthorityDetails, id);
    }
}