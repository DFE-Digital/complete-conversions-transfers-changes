using Dfe.Complete.Application.LocalAuthorities.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.LocalAuthorities
{
    public class DeleteLocalAuthorityModel(ISender sender) : ServiceSupportModel(LocalAuthoriesNavigation)
    {
        public required string Name { get; set; }

        [BindProperty(SupportsGet = true, Name = "id")]
        public required Guid Id { get; set; }

        public void OnGet()
        {
            Name = TempData["LA_Name"] as string ?? string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await sender.Send(new DeleteLocalAuthorityCommand(new LocalAuthorityId(Id)));
            if (response.IsSuccess)
            {
                TempData["HasDeletedLa"] = true;
                return RedirectToPage(Links.LocalAuthorities.ListLocalAuthorities);
            }
            return Page(); 
        }
    }
}