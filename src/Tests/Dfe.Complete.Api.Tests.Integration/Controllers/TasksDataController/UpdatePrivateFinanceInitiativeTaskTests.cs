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
    public class UpdatePrivateFinanceInitiativeTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdatePrivateFinanceInitiativeTaskAsync_ShouldUpdate_ConversionTaskData(
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

            var command = new UpdatePrivateFinanceInitiativeTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                NotApplicable = false,
                SupplementaryFundingAgreementPfiClausesInserted = true,
                MasterFundingAgreementPfiClausesInserted = true,
                Received = true,
                Cleared = true,
                DraftSaved = true,
                SignedByAllStakeHolders = true,
                DocumentsSentToSOPUForClearance = true,
                FinalVersionSavedInSharepointFolder = true
            };

            // Act
            await tasksDataClient.UpdatePrivateFinanceInitiativeTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.PrivateFinanceInitiativeNotApplicable);
            Assert.True(existingTaskData.PrivateFinanceInitiativeSupplementaryFundingAgreementPfiClausesInserted);
            Assert.True(existingTaskData.PrivateFinanceInitiativeMasterFundingAgreementPfiClausesInserted);
            Assert.True(existingTaskData.PrivateFinanceInitiativeReceived);
            Assert.True(existingTaskData.PrivateFinanceInitiativeCleared);
            Assert.True(existingTaskData.PrivateFinanceInitiativeDraftSavedInTrustSharepointFolder);
            Assert.True(existingTaskData.PrivateFinanceInitiativeSignedByAllStakeholders);
            Assert.True(existingTaskData.PrivateFinanceInitiativeFinalVersionSavedInSharepointFolder);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdatePrivateFinanceInitiativeTaskAsync_ShouldUpdateNotApplicableOnly(
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

            var command = new UpdatePrivateFinanceInitiativeTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                NotApplicable = true,
                SupplementaryFundingAgreementPfiClausesInserted = true,
                MasterFundingAgreementPfiClausesInserted = true,
                Received = true,
                Cleared = true,
                SignedByAllStakeHolders = true,
                DocumentsSentToSOPUForClearance = true,
                FinalVersionSavedInSharepointFolder = true
            };

            // Act
            await tasksDataClient.UpdatePrivateFinanceInitiativeTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.PrivateFinanceInitiativeNotApplicable);
            Assert.Null(existingTaskData.PrivateFinanceInitiativeSupplementaryFundingAgreementPfiClausesInserted);
            Assert.Null(existingTaskData.PrivateFinanceInitiativeMasterFundingAgreementPfiClausesInserted);
            Assert.Null(existingTaskData.PrivateFinanceInitiativeReceived);
            Assert.Null(existingTaskData.PrivateFinanceInitiativeCleared);
            Assert.Null(existingTaskData.PrivateFinanceInitiativeDraftSavedInTrustSharepointFolder);
            Assert.Null(existingTaskData.PrivateFinanceInitiativeSignedByAllStakeholders);
            Assert.Null(existingTaskData.PrivateFinanceInitiativeFinalVersionSavedInSharepointFolder);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdatePrivateFinanceInitiativeTaskAsync_ShouldFail_WhenTaskDataIdNotMatched(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var command = new UpdatePrivateFinanceInitiativeTaskCommand
            {
                TaskDataId = new TaskDataId { Value = Guid.NewGuid() }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => tasksDataClient.UpdatePrivateFinanceInitiativeTaskAsync(command, default));

            Assert.Contains($"Conversion task data TaskDataId {{ Value = {command.TaskDataId.Value} }} not found.", exception.Message);
        }
    }
}
