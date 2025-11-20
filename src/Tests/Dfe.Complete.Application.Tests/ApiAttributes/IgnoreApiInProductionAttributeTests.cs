using Dfe.Complete.Application.ApiAttributes;
using Dfe.Complete.Application.ApiConfig;

namespace Dfe.Complete.Application.Tests.ApiAttributes
{
    public class IgnoreApiInProductionAttributeTests
    {
        [Fact]
        public void Constructor_SetsIgnoreApi_WhenIsProductionIsTrue()
        {
            // Arrange
            IgnoreApiInProductionConfig.IsProduction = true;
            var attr = new IgnoreApiInProductionAttribute();

            Assert.True(attr.IgnoreApi);
        }

        [Fact]
        public void Constructor_DoesNotSetIgnoreApi_WhenIsProductionIsFalse()
        {
            // Arrange
            IgnoreApiInProductionConfig.IsProduction = false;
            var attr = new IgnoreApiInProductionAttribute();

            Assert.False(attr.IgnoreApi);
        }
    }
}
