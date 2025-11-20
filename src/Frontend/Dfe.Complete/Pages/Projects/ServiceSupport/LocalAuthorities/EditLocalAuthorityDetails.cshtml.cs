using Dfe.Complete.Application.LocalAuthorities.Commands;
using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.LocalAuthorities.Queries;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Models.LocalAuthority;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using ValidationConstants = Dfe.Complete.Constants.ValidationConstants;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.LocalAuthorities
{
    public class EditLocalAuthorityDetailsModel(ISender sender, IErrorService errorService) : LocalAuthorityAddEditBaseModel()
    {
        [BindProperty(SupportsGet = true, Name = nameof(Id))]
        public required string Id { get; set; }
       
        [BindProperty(Name = nameof(ContactId))]
        public Guid? ContactId { get; set; }

        public async Task OnGetAsync()
        {
            var localAuthorityResponse = await sender.Send(new GetLocalAuthorityDetailsQuery(new LocalAuthorityId(new Guid(Id))));
            SetLocalAuthorityDetails(localAuthorityResponse.Value!);
        }
        private void SetLocalAuthorityDetails(LocalAuthorityDetailsModel model)
        {
            Code = model.LocalAuthority.Code;
            Name = model.LocalAuthority.Name;
            Address1 = model.LocalAuthority.Address1;
            Address2 = model.LocalAuthority.Address2;
            Address3 = model.LocalAuthority.Address3;
            AddressTown = model.LocalAuthority.AddressTown;
            AddressCounty = model.LocalAuthority.AddressCounty;
            AddressPostcode = model.LocalAuthority.AddressPostcode;
            if (model.Contact != null)
            {
                Title = model.Contact.Title;
                ContactName = model.Contact.Name;
                Email = model.Contact.Email;
                Phone = model.Contact.Phone;
                ContactId = model.Contact.Id.Value;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }
            //get contact id. 
            var contactId = ContactId.HasValue ? new ContactId(ContactId.Value) : new ContactId(Guid.NewGuid());
            var response = await sender.Send(new UpdateLocalAuthorityCommand(new LocalAuthorityId(new Guid(Id)), Code, Address1,
                Address2, Address3, AddressTown, AddressCounty, AddressPostcode.ToUpper(), contactId, Title, ContactName, Email, Phone));
            if (response.IsSuccess)
            {
                TempData["HasUpdatedLaDetails"] = true;
                return RedirectToPage(Links.LocalAuthorities.ViewLocalAuthorityDetails.Page, new { Id });
            }
            else if (response.Error == string.Format(ErrorMessagesConstants.AlreadyExistedLocalAuthorityWithCode, Code))
            {
                errorService.AddError(nameof(Code), ValidationConstants.AlreadyBeenTaken);
                ModelState.AddModelError(nameof(Code), ValidationConstants.AlreadyBeenTaken);
            }
            return Page();
        }
        public string GetLocalAuthorityDetailsUrl(string id)
            => string.Format(RouteConstants.LocalAuthorityDetails, id);
    }
}
