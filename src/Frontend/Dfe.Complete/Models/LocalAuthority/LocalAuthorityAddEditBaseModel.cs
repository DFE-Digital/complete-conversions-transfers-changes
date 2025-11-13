using Dfe.Complete.Constants;
using Dfe.Complete.Pages.Projects.ServiceSupport;
using Dfe.Complete.Validators;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Models.LocalAuthority
{
    [ExcludeFromCodeCoverage]
    public class LocalAuthorityAddEditBaseModel() : ServiceSupportModel(LocalAuthoriesNavigation)
    {
        [BindProperty(Name = nameof(Code))]
        [Required(ErrorMessage = ValidationConstants.FullNameRequiredMessage)]
        public string Code { get; set; } = null!;

        [BindProperty(Name = nameof(Name))]
        public string? Name { get; set; }

        [BindProperty(Name = nameof(Address1))]
        [Required(ErrorMessage = ValidationConstants.LocalAuthorityAddressLine1Required)]
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
        [Required(ErrorMessage = ValidationConstants.LocalAuthorityPostcodeRequired)]
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
    }
}
