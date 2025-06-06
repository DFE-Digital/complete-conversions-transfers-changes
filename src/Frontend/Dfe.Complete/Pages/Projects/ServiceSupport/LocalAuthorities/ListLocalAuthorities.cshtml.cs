using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.LocalAuthorities.Queries;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Pages.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.LocalAuthorities
{
    [Authorize(policy: UserPolicyConstants.ManagerLocalAuthorities)]
    public class ListLocalAuthoritiesModel(ISender sender) : ServiceSupportModel(LocalAuthoriesNavigation)
    { 
        public List<LocalAuthorityQueryModel> LocalAuthorities { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync()
        {
            var localAuthoriesResponse = await sender.Send(new ListLocalAuthoritiesQuery() {  Page = PageNumber - 1, Count = PageSize });
            LocalAuthorities = localAuthoriesResponse.Value ?? []; 

            Pagination = new PaginationModel(RouteConstants.ListLocalAuthorities, PageNumber, localAuthoriesResponse.ItemCount, PageSize);
            var hasPageFound = HasPageFound(Pagination.IsOutOfRangePage);
            return hasPageFound ?? Page();
        }

        public string GetLocalAuthorityDetailsUrl(string id)
            => string.Format(RouteConstants.LocalAuthorityDetails, id);
    }
}
