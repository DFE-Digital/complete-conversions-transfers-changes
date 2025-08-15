using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController
{
    public class ConversionTaskDataControllerTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetConversionTasksDataByIdAsync_ShouldReturn_A_TaskDaa(
        CustomWebApplicationDbContextFactory<Program> factory,
        IConversionTasksDataClient conversionTasksDatClient,
        IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var task = fixture.Build<ConversionTasksData>()
                .With(tt => tt.Id, new Domain.ValueObjects.TaskDataId(Guid.NewGuid()))
                .Create();

            await dbContext.ConversionTasksData.AddAsync(task);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await conversionTasksDatClient.GetConversionTasksDataByIdAsync(task.Id.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id.Value, result.Id!.Value);
        }
    }
}
