using System.Security.Claims;
using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController
{
    public class ProjectGroupControllerTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetProjectGroupByIdAsync_ShouldReturn_A_ProjectGroup(
            CustomWebApplicationDbContextFactory<Program> factory,
            IProjectGroupClient projectGroupClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var projectGroup = fixture.Build<ProjectGroup>()
               //.With(pg => pg.Id, new Domain.ValueObjects.ProjectGroupId(Guid.NewGuid()))
               //.With(pg => pg.GroupIdentifier, "test-group-identifier")
               //.With(pg => pg.TrustUkprn, new Domain.ValueObjects.Ukprn(123456))
               .Create();

            await dbContext.ProjectGroups.AddAsync(projectGroup);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await projectGroupClient.GetProjectGroupByIdAsync(projectGroup.Id.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectGroup.Id.Value, result.Id!.Value);
        }
    }
}
