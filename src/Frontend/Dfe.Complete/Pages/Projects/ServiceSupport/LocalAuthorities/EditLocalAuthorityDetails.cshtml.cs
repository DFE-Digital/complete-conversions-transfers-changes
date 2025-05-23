using Dfe.Complete.Application.LocalAuthorities.Commands;
using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.LocalAuthorities.Queries;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.LocalAuthorities
{
    public class EditLocalAuthorityDetailsModel(ISender sender, IErrorService errorService) : ServiceSupportModel(LocalAuthoriesNavigation)
    {
        [BindProperty(SupportsGet = true, Name = nameof(Id))]
        public required string Id { get; set; }

        [BindProperty(Name = nameof(Code))] 
        [Required(ErrorMessage = ValidationConstants.CannotBeBlank)]
        public string Code { get; set; } = null!;
        [BindProperty(Name = nameof(Name))]
        public string? Name { get; set; }
        [BindProperty(Name = nameof(Address1))]
        [Required(ErrorMessage = ValidationConstants.CannotBeBlank)]
        public string Address1 { get; set; } = null!;
        [BindProperty(Name = nameof(Address2))]
        public string? Address2 { get; set; }
        [BindProperty(Name = nameof(Address3))]
        public string? Address3 { get; set; }
        [BindProperty(Name = nameof(AddressTown))]
        public string? AddressTown { get; set; }
        [BindProperty(Name = nameof(AddressCounty))]
        public string? AddressCounty { get; set; }
        [BindProperty(Name = nameof(AddressPostcode))]
        [Required(ErrorMessage = ValidationConstants.CannotBeBlank)]
        [RegularExpression(ValidationExpressions.UKPostCode, ErrorMessage = ValidationConstants.NotRecognisedUKPostcode)]
        public string AddressPostcode { get; set; } = null!;
        [BindProperty(Name = nameof(Title))]
        public string? Title { get; set; }
        [BindProperty(Name = nameof(ContactName))]
        public string? ContactName { get; set; }
        [BindProperty(Name = nameof(Email))]
        [EmailValidation(ErrorMessage = ValidationConstants.InvalidEmailFormat)]
        public string? Email { get; set; }
        [BindProperty(Name = nameof(Phone))]
        [ValidUkPhoneNumber(ErrorMessage = ValidationConstants.NotRecognisedUKPhone)]
        public string? Phone { get; set; }
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
            else { ContactId = Guid.NewGuid(); }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }
            //get contact id. 
            var contactId = ContactId.HasValue ? new ContactId(ContactId.Value) : null;
            var response = await sender.Send(new UpdateLocalAuthorityCommand(new LocalAuthorityId(new Guid(Id)), Code, Address1,
                Address2, Address3, AddressTown, AddressCounty, AddressPostcode, contactId, Title, ContactName, Email, Phone));
            if (response.IsSuccess)
            {
                TempData["HasUpdatedLaDetails"] = true;
                return RedirectToPage(Links.LocalAuthorities.ViewLocalAuthorityDetails.Page, new { Id });
            }
            return Page();
        }
        public string GetLocalAuthorityDetailsUrl(string id)
            => string.Format(RouteConstants.LocalAuthorityDetails, id);
    }
}
