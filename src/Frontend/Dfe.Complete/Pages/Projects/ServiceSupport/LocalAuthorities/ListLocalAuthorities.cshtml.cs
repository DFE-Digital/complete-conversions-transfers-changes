using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.LocalAuthorities.Queries;
using Dfe.Complete.Constants;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.LocalAuthorities
{
    public class ListLocalAuthoritiesModel(ISender sender) : ServiceSupportModel(LocalAuthoriesNavigation)
    { 
        public List<LocalAuthorityQueryModel> LocalAuthorities { get; set; } = default!;
        public async Task OnGetAsync()
        {
            var localAuthoriesResponse = await sender.Send(new GetLocalAuthoritiesQuery(PageNumber - 1, PageSize));
            LocalAuthorities = localAuthoriesResponse.Value ?? []; 

            Pagination = new PaginationModel(RouteConstants.ListLocalAuthorities, PageNumber, localAuthoriesResponse.ItemCount, PageSize); 
        } 
    }
}
