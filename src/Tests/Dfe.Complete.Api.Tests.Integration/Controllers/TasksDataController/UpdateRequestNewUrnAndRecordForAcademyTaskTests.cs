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
    public class UpdateRequestNewUrnAndRecordForAcademyTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateRequestNewUrnAndRecordForAcademyTaskAsync_ShouldUpdate_TransferTaskData(
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

            var command = new UpdateRequestNewUrnAndRecordForAcademyTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                NotApplicable = null,
                Complete = true,
                Receive = true,
                Give = true,
            };

            // Act
            await tasksDataClient.UpdateRequestNewUrnAndRecordForAcademyTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.RequestNewUrnAndRecordNotApplicable);
            Assert.True(existingTaskData.RequestNewUrnAndRecordComplete);
            Assert.True(existingTaskData.RequestNewUrnAndRecordReceive);
            Assert.True(existingTaskData.RequestNewUrnAndRecordGive);

        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateRequestNewUrnAndRecordForAcademyTaskAsync_ShouldUpdateNotApplicableOnly(
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

            var command = new UpdateRequestNewUrnAndRecordForAcademyTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                NotApplicable = true,
                Complete = true,
                Receive = true,
                Give = true,
            };

            // Act
            await tasksDataClient.UpdateRequestNewUrnAndRecordForAcademyTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.RequestNewUrnAndRecordComplete);
            Assert.Null(existingTaskData.RequestNewUrnAndRecordGive);
            Assert.Null(existingTaskData.RequestNewUrnAndRecordReceive);
            Assert.True(existingTaskData.RequestNewUrnAndRecordNotApplicable);
        }
    }
}