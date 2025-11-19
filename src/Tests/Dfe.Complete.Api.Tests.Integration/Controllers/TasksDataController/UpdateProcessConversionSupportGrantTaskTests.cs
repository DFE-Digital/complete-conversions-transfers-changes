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
    public class UpdateProcessConversionSupportGrantTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateProcessConversionSupportGrantTaskAsync_ShouldUpdate_ConversionTaskData(
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

            var command = new UpdateProcessSupportGrantTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },              
                ConversionGrantCheckVendorAccount = true,
                ConversionGrantPaymentForm = true,
                ConversionGrantSendInformation = false,
                ConversionGrantSharePaymentDate = false
            };

            // Act
            await tasksDataClient.UpdateProcessConversionSupportGrantTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.ConversionGrantNotApplicable);
            Assert.True(existingTaskData.ConversionGrantCheckVendorAccount);
            Assert.True(existingTaskData.ConversionGrantPaymentForm);
            Assert.False(existingTaskData.ConversionGrantSendInformation);
            Assert.False(existingTaskData.ConversionGrantSharePaymentDate);           
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateProcessConversionSupportGrantTaskAsync_ShouldNotUpdate_IfNotApplicableIsTrue(
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

            var command = new UpdateProcessSupportGrantTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                NotApplicable = true,
                ConversionGrantCheckVendorAccount = true,
                ConversionGrantPaymentForm = true,
                ConversionGrantSendInformation = true,
                ConversionGrantSharePaymentDate = true
            };            

            // Act
            await tasksDataClient.UpdateProcessConversionSupportGrantTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ConversionGrantNotApplicable);
            Assert.Null(existingTaskData.ConversionGrantCheckVendorAccount);
            Assert.Null(existingTaskData.ConversionGrantPaymentForm);
            Assert.Null(existingTaskData.ConversionGrantSendInformation);
            Assert.Null(existingTaskData.ConversionGrantSharePaymentDate);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateProcessConversionSupportGrantTaskAsync_ShouldFail_WhenTaskDataIdNotMatched(
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

            var command = new UpdateProcessSupportGrantTaskCommand
            {
                TaskDataId = new TaskDataId { Value = Guid.NewGuid() }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => tasksDataClient.UpdateProcessConversionSupportGrantTaskAsync(command, default));

            Assert.Contains($"Conversion task data TaskDataId {{ Value = {command.TaskDataId.Value} }} not found.", exception.Message);
        }
    }
}
