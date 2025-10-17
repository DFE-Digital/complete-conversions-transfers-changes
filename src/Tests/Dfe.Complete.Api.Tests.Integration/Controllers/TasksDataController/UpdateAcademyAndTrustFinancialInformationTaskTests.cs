using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory; 
using System.Net;
using System.Security.Claims; 
using Microsoft.EntityFrameworkCore;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{ 
    public class UpdateAcademyAndTrustFinancialInformationTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateAcademyAndTrustFinancialInformationTaskAsync_ShouldUpdate_TransferTaskData(
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

            var command = new UpdateAcademyAndTrustFinancialInformationTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                NotApplicable = null,
                AcademySurplusOrDeficit = "surplus",
                TrustSurplusOrDeficit = "deficit"
            };

            // Act
            await tasksDataClient.UpdateAcademyAndTrustFinancialInformationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Equal(command.AcademySurplusOrDeficit, existingTaskData.CheckAndConfirmFinancialInformationAcademySurplusDeficit);
            Assert.Equal(command.TrustSurplusOrDeficit, existingTaskData.CheckAndConfirmFinancialInformationTrustSurplusDeficit);
            Assert.Null(existingTaskData.CheckAndConfirmFinancialInformationNotApplicable);
        }
         

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateAcademyAndTrustFinancialInformationTaskAsync_ShouldUpdateNotApplicableOnly(
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

            var command = new UpdateAcademyAndTrustFinancialInformationTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                NotApplicable = true,
                AcademySurplusOrDeficit = "surplus",
                TrustSurplusOrDeficit = "deficit"
            };

            // Act
            await tasksDataClient.UpdateAcademyAndTrustFinancialInformationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.CheckAndConfirmFinancialInformationTrustSurplusDeficit);
            Assert.Null(existingTaskData.CheckAndConfirmFinancialInformationAcademySurplusDeficit);
            Assert.True(existingTaskData.CheckAndConfirmFinancialInformationNotApplicable);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateAcademyAndTrustFinancialInformationTaskAsync_ShouldFail_WhenTaskDataIdNotMatched(
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

            var command = new UpdateAcademyAndTrustFinancialInformationTaskCommand
            {
                TaskDataId = new TaskDataId { Value = Guid.NewGuid() }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => tasksDataClient.UpdateAcademyAndTrustFinancialInformationTaskAsync(command, default));
              
            Assert.Contains($"Transfer task data TaskDataId {{ Value = { command.TaskDataId.Value} }} not found.", exception.Message);
        }
    }
}
