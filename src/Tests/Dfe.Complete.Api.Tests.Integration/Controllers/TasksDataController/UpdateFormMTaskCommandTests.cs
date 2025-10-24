using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory; 
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using UpdateFormMTaskCommand = Dfe.Complete.Client.Contracts.UpdateFormMTaskCommand;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{ 
    public class UpdateFormMTaskCommandTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateFormMTaskAsync_ShouldUpdate_TransferTaskData(
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
            
            var command = new UpdateFormMTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                Received = true,
                ReceivedTitlePlans = true,
                Cleared = true,
                Signed = true,
                Saved = true
            };
            
            // Act
            await tasksDataClient.UpdateFormMTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.FormMReceivedFormM);
            Assert.True(existingTaskData.FormMReceivedTitlePlans);
            Assert.True(existingTaskData.FormMCleared);
            Assert.True(existingTaskData.FormMSigned);
            Assert.True(existingTaskData.FormMSaved);
            Assert.Null(existingTaskData.FormMNotApplicable);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateFormMTaskAsync_ShouldUpdateNotApplicableOnly(
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

            var command = new UpdateFormMTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                Received = true,
                ReceivedTitlePlans = true,
                Cleared = true,
                Saved = true,
                Signed = true,
                NotApplicable = true
            };

            // Act
            await tasksDataClient.UpdateFormMTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.FormMReceivedFormM);
            Assert.Null(existingTaskData.FormMReceivedTitlePlans);
            Assert.Null(existingTaskData.FormMCleared);
            Assert.Null(existingTaskData.FormMSaved);
            Assert.Null(existingTaskData.FormMSigned);
            Assert.True(existingTaskData.FormMNotApplicable);
        }
    }
}
