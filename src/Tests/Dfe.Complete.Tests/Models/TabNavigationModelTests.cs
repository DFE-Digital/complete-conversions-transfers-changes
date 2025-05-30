
using Dfe.Complete.Models;

namespace Dfe.Complete.Tests.Models;

public class TabNavigationModelTests
{
    [Fact]
    public void Constructor_WhenReceivesTabName_ShouldHaveCorrectCurrentTabName()
    {
        var tabNavigationModel = new TabNavigationModel("my-current-tab");

        Assert.Equal("my-current-tab", tabNavigationModel.CurrentTab);
    }
}
