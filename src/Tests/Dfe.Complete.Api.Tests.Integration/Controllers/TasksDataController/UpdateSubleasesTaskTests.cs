using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Utils.Exceptions;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{
    public class UpdateSubleasesTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateSubleasesTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();
            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            var command = new UpdateSubleasesTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                Cleared = true,
                Received = true,
                Saved = true,
                Signed = false,
                EmailSigned = false,
                ReceiveSigned = true,
                SaveSigned = false,
            };

            // Act
            await tasksDataClient.UpdateSubleasesTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.SubleasesNotApplicable);
            Assert.True(existingTaskData.SubleasesCleared);
            Assert.True(existingTaskData.SubleasesReceived);
            Assert.False(existingTaskData.SubleasesSigned);
            Assert.True(existingTaskData.SubleasesSaved);
            Assert.False(existingTaskData.SubleasesEmailSigned);
            Assert.True(existingTaskData.SubleasesReceiveSigned);
            Assert.False(existingTaskData.SubleasesSaveSigned);
        }
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateSubleasesTaskAsync_ShouldNotUpdate_IfNotApplicableIsTrue(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();
            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            var command = new UpdateSubleasesTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                NotApplicable = true,
                Cleared = true,
                Received = true,
                Saved = true,
                Signed = true,
                EmailSigned = true,
                ReceiveSigned = true,
                SaveSigned = true,
            };

            // Act
            await tasksDataClient.UpdateSubleasesTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.SubleasesNotApplicable);
            Assert.Null(existingTaskData.SubleasesCleared);
            Assert.Null(existingTaskData.SubleasesReceived);
            Assert.Null(existingTaskData.SubleasesSigned);
            Assert.Null(existingTaskData.SubleasesSaved);
            Assert.Null(existingTaskData.SubleasesEmailSigned);
            Assert.Null(existingTaskData.SubleasesReceiveSigned);
            Assert.Null(existingTaskData.SubleasesSaveSigned);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateSubleasesTaskAsync_ShouldFail_WhenTaskDataIdNotMatched(
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

            var command = new UpdateSubleasesTaskCommand
            {
                TaskDataId = new TaskDataId { Value = Guid.NewGuid() }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => tasksDataClient.UpdateSubleasesTaskAsync(command, default));

            Assert.Contains($"Conversion task data TaskDataId {{ Value = {command.TaskDataId.Value} }} not found.", exception.Message);
        }
    }
}
