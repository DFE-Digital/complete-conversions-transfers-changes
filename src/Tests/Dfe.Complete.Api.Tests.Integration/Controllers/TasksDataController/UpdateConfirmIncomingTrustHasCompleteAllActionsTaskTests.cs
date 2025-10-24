using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{
    public class UpdateConfirmIncomingTrustHasCompleteAllActionsTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateConfirmIncomingTrustHasCompleteAllActionsTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            var command = new UpdateConfirmIncomingTrustHasCompleteAllActionsTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                Emailed = true,
                Saved = true,
            };

            // Act
            await tasksDataClient.UpdateConfirmIncomingTrustHasCompleteAllActionsTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ConfirmIncomingTrustHasCompletedAllActionsEmailed);
            Assert.True(existingTaskData.ConfirmIncomingTrustHasCompletedAllActionsSaved);
        }
    }
}