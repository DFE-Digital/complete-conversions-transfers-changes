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
    public class UpdateConfirmAcademyRiskProtectionArrangementsTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateConfirmAcademyRiskProtectionArrangementsTaskAsync_ShouldUpdate_TransferTaskData(
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

            var command = new UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Transfer,
                RpaPolicyConfirm = true
            };

            // Act
            await tasksDataClient.UpdateConfirmAcademyRiskProtectionArrangementsTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.RpaPolicyConfirm);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateConfirmAcademyRiskProtectionArrangementsTaskAsync_ShouldUpdate_ConversionTaskData(
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

            var command = new UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Conversion,
                RpaOption = RiskProtectionArrangementOption.Commercial,
                RpaReason = "Some reason"
            };

            // Act
            await tasksDataClient.UpdateConfirmAcademyRiskProtectionArrangementsTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Equal(command.RpaOption.ToString(), existingTaskData.RiskProtectionArrangementOption.ToString());
            Assert.Equal(command.RpaReason, existingTaskData.RiskProtectionArrangementReason);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateConfirmAcademyRiskProtectionArrangementsTaskAsync_ShouldFail_WhenProjectTypeOmitted(
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

            var command = new UpdateConfirmAcademyRiskProtectionArrangementsTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<CompleteApiException>(() => tasksDataClient.UpdateConfirmAcademyRiskProtectionArrangementsTaskAsync(command, default));

            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

            var validationErrors = exception.Response;
            Assert.NotNull(validationErrors);
            Assert.Contains("The ProjectType field is required.", validationErrors);
        }
    }
}
