using Dfe.Complete.Application.LocalAuthorities.Commands;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Models.LocalAuthority;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ValidationConstants = Dfe.Complete.Constants.ValidationConstants;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.LocalAuthorities
{
    [Authorize(policy: UserPolicyConstants.ManageLocalAuthorities)]
    public class CreateNewLocalAuthorityModel(ISender sender, IErrorService errorService) : LocalAuthorityAddEditBaseModel()
    {  
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState.Keys, ModelState);
                return Page();
            }

            var response = await sender.Send(new CreateLocalAuthorityCommand(Code, Name, Address1, Address2, Address3,
                AddressTown, AddressCounty, AddressPostcode.ToUpper(), Title!, ContactName!, Email, Phone));

            if (response.IsSuccess && response.Value != null)
            {
                TempData["HasCreatedLaDetails"] = true;
                return RedirectToPage(Links.LocalAuthorities.ViewLocalAuthorityDetails.Page, new { Id = response.Value.LocalAuthorityId.Value });
            }
            else if (response.Error == string.Format(ErrorMessagesConstants.AlreadyExistedLocalAuthorityWithCode, Code))
            {
                errorService.AddError(nameof(Code), ValidationConstants.AlreadyBeenTaken);
                ModelState.AddModelError(nameof(Code), ValidationConstants.AlreadyBeenTaken);
            }
            return Page();
        }
    }
}
