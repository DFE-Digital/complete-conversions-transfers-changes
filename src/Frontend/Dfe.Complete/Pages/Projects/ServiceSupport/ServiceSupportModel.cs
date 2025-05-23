using Dfe.Complete.Constants;
using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.Projects.ServiceSupport
{
    public class ServiceSupportModel(string currentSubNavigationItem) : BaseProjectsPageModel(currentSubNavigationItem)
    {

        protected TabNavigationModel AllProjectsTabNavigationModel = new(TabNavigationModel.AllProjectsTabName);

        public const string ConversionURNsNavigation = "conversion-urns";
        public const string LocalAuthoriesNavigation = "local-authorites";
        public const string UsersNavigation = "users";
        public string CurrentSubNavigationItem { get; set; } = currentSubNavigationItem;

        public string EditLocalAuthorityUrl(string id) 
            => string.Format(RouteConstants.EditLocalAuthorityDetails, id);
        public string GetLocalAuthorityDetailsUrl(string id)
            => string.Format(RouteConstants.LocalAuthorityDetails, id);
    }
}
