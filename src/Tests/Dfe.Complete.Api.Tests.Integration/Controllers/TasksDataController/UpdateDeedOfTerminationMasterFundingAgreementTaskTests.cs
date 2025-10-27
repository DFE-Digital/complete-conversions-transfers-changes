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
    public class UpdateDeedOfTerminationMasterFundingAgreementTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateDeedOfTerminationMasterFundingAgreementTaskAsync_ShouldUpdate_TransferTaskData(
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

            var command = new UpdateDeedOfTerminationMasterFundingAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },                
                Received = true,
                Cleared = true,                
                Saved = true,
                Signed = true,
                ContactFinancialReportingTeam = true,
                SignedSecretaryState = true,
                SavedAcademySharePointHolder = true,
            };

            // Act
            await tasksDataClient.UpdateDeedOfTerminationMasterFundingAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementReceived);
            Assert.True(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementCleared);
            Assert.True(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementSigned);
            Assert.True(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint);
            Assert.True(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam);
            Assert.True(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState);
            Assert.True(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder);            
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateDeedOfTerminationMasterFundingAgreementTaskAsync_ShouldUpdateNotApplicableOnly(
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

            var command = new UpdateDeedOfTerminationMasterFundingAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                Received = true,
                Cleared = true,
                Saved = true,
                Signed = true,
                ContactFinancialReportingTeam = true,
                SignedSecretaryState = true,
                SavedAcademySharePointHolder = true,
                NotApplicable = true
            };

            // Act
            await tasksDataClient.UpdateDeedOfTerminationMasterFundingAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementReceived);
            Assert.Null(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementCleared);
            Assert.Null(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementSigned);
            Assert.Null(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint);
            Assert.Null(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam);
            Assert.Null(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState);
            Assert.Null(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder);
            Assert.True(existingTaskData.DeedOfTerminationForTheMasterFundingAgreementNotApplicable);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateDeedOfTerminationMasterFundingAgreementTaskAsync_ShouldFail_WhenTaskDataIdNotMatched(
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

            var command = new UpdateDeedOfTerminationMasterFundingAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = Guid.NewGuid() }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => tasksDataClient.UpdateDeedOfTerminationMasterFundingAgreementTaskAsync(command, default));

            Assert.Contains($"Transfer task data TaskDataId {{ Value = {command.TaskDataId.Value} }} not found.", exception.Message);
        }
    }
}