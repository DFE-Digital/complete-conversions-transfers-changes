
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.List.ProjectsInProgress;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Models;

public class ConversionOrTransferInProgressModelTests
{
    [Fact]
    public void Constructor_WhenReceivesArgs_ShouldHaveCorrectProperties()
    {
        var model = new ConversionOrTransferInProgressModel("my-current-tab", ProjectType.Transfer);

        Assert.Equal("my-current-tab", model.CurrentSubNavigationItem);
        Assert.Equal(ProjectType.Transfer, model.ProjectType);
        Assert.Null(model.Projects);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public void ProjectsPropertyIsSet_WhenProjectsAreSet(IFixture fixture)
    {
        var expectedProjects = fixture.CreateMany<ListAllProjectsResultModel>(5).ToList();
        var model = new ConversionOrTransferInProgressModel("my-current-tab", ProjectType.Transfer)
        {
            Projects = expectedProjects
        };

        Assert.NotNull(model.Projects);

        for (var i = 0; i < expectedProjects.Count; i++)
        {
            Assert.Equal(expectedProjects[i].ProjectId.Value, model.Projects[i].ProjectId.Value);
        }
    }
}
