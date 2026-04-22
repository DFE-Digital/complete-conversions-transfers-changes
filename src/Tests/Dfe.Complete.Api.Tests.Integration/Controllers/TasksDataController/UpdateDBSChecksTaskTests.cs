using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;


namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{

    public class UpdateDBSChecksTaskTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateConfirmDbsChecksTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims =
            [
                new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole),
                new Claim(ClaimTypes.Role, ApiRoles.WriteRole)
            ];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            var command = new UpdateConfirmDbsChecksTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ConfirmDBSChecks = true
            };

            // Act
            await tasksDataClient.UpdateConfirmDbsChecksTaskAsync(command, default);
            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);

            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ConfirmDBSChecks);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateConfirmDbsChecksTaskAsync_ShouldSetToFalse_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims =
            [
                new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole),
                new Claim(ClaimTypes.Role, ApiRoles.WriteRole)
            ];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            taskData.ConfirmDBSChecks = true; // Set initial value to true
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            var command = new UpdateConfirmDbsChecksTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ConfirmDBSChecks = false
            };

            // Act
            await tasksDataClient.UpdateConfirmDbsChecksTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.ConfirmDBSChecks);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateConfirmDbsChecksTaskAsync_WithUnauthorizedUser_ShouldReturnUnauthorized(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)]; // Only read role, not write

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            var command = new UpdateConfirmDbsChecksTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ConfirmDBSChecks = true
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CompleteApiException>(() =>
                tasksDataClient.UpdateConfirmDbsChecksTaskAsync(command, default));

            Assert.Equal(403, exception.StatusCode);
        }

    }
}
