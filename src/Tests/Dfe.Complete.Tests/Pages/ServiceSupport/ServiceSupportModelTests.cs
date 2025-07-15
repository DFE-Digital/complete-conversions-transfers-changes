using Dfe.Complete.Pages.Projects.ServiceSupport;

namespace Dfe.Complete.Tests.Pages.ServiceSupport
{
    public class ServiceSupportModelTests
    {
        [Fact]
        public void Constructor_SetsCurrentSubNavigationItem()
        {
            // Arrange
            var expectedSubNavItem = "conversion-urns";

            // Act
            var model = new ServiceSupportModel(expectedSubNavItem);

            // Assert
            Assert.Equal(expectedSubNavItem, model.CurrentSubNavigationItem);
        }

        [Fact]
        public void Constants_HaveExpectedValues()
        {
            // Assert
            Assert.Equal("conversion-urns", ServiceSupportModel.ConversionURNsNavigation);
            Assert.Equal("local-authorites", ServiceSupportModel.LocalAuthoriesNavigation);
        }
    }
}
