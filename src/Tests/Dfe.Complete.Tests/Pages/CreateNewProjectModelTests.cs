using Dfe.Complete.Pages.Projects;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Tests.Pages.Projects
{
    public class CreateNewProjectModelTests
    {
        [Theory]
        [InlineData("conversion", "/Projects/Conversion/CreateNewProject")]
        [InlineData("fam_conversion", "/Projects/MatConversion/CreateNewProject")]
        [InlineData("transfer", "/Projects/Transfer/CreateNewProject")]
        [InlineData("fam_transfer", "/Projects/MatTransfer/CreateNewProject")]
        public void OnPost_RedirectsToCorrectPage_BasedOnProjectType(string projectType, string expectedRedirectPage)
        {
            // Arrange
            var model = new CreateNewProjectModel { ProjectType = projectType };

            // Act
            var result = model.OnPost() as RedirectToPageResult;

            // Assert
            if (string.IsNullOrEmpty(expectedRedirectPage))
            {
                Assert.Null(result);
            }
            else
            {
                Assert.NotNull(result);
                Assert.Equal(expectedRedirectPage, result.PageName);
            }
        }
    }
}
