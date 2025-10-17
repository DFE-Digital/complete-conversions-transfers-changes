using Dfe.Complete.Application.LocalAuthorities.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Validators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using ValidationConstants = Dfe.Complete.Constants.ValidationConstants;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.LocalAuthorities
{
    [Authorize(policy: UserPolicyConstants.ManageLocalAuthorities)]
    public class CreateNewLocalAuthorityModel(ISender sender, IErrorService errorService) : ServiceSupportModel(LocalAuthoriesNavigation)
    {
        [BindProperty(Name = nameof(Name))]
        [Required(ErrorMessage = ValidationConstants.CannotBeBlank)]
        public string Name { get; set; } = null!;
        [BindProperty(Name = nameof(Code))]
        [Required(ErrorMessage = ValidationConstants.CannotBeBlank)]
        public string Code { get; set; } = null!;
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }
            var localAuthorityId = new LocalAuthorityId(Guid.NewGuid());
            var response = await sender.Send(new CreateLocalAuthorityCommand(Code, Name, Address1, Address2, Address3,
                AddressTown, AddressCounty, AddressPostcode.ToUpper(), new ContactId(Guid.NewGuid()), Title!, ContactName!, Email, Phone));

            if (response.IsSuccess)
            {
                TempData["HasCreatedLaDetails"] = true;
                return RedirectToPage(Links.LocalAuthorities.ViewLocalAuthorityDetails.Page, new { Id = localAuthorityId.Value });
            }
            else if(response.Error == string.Format(ErrorMessagesConstants.AlreadyExistedLocalAuthorityWithCode, Code))
            { 
                errorService.AddError(nameof(Code), ValidationConstants.AlreadyBeenTaken);
                ModelState.AddModelError(nameof(Code), ValidationConstants.AlreadyBeenTaken);
            }
            return Page();
        }
    }
}
