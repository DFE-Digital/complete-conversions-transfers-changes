using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.LocalAuthorities.Queries;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.ServiceSupport.LocalAuthorities
{
    public class ViewLocalAuthorityDetailsModel(ISender sender) : ServiceSupportModel(LocalAuthoriesNavigation)
    {
        [BindProperty(SupportsGet = true, Name = "id")]
        public required string Id { get; set; }

        public required LocalAuthorityDetailsModel Details { get; set; }

        public async Task OnGetAsync()
        {
            var localAuthorityResponse = await sender.Send(new GetLocalAuthorityDetailsQuery(new LocalAuthorityId(new Guid(Id))));
            Details = localAuthorityResponse?.Value!;
            TempData["LA_Name"] = Details.LocalAuthority.Name;
            TempData["LA_ContatId"] = Details.Contact?.Id.Value;
        }

        public string DeleteLocalAuthorityUrl() => string.Format(RouteConstants.DeleteLocalAuthorityDetails, Id);
        public string EditLocalAuthorityUrl(string id)
            => string.Format(RouteConstants.EditLocalAuthorityDetails, id);
    }
}
