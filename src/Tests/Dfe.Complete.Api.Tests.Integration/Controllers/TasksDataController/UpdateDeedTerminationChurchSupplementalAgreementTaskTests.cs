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
    public class UpdateDeedTerminationChurchSupplementalAgreementTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateDeedTerminationChurchSupplementalAgreementTaskAsync_ShouldUpdate_TransferTaskData(
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

            var command = new UpdateDeedTerminationChurchSupplementalAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value }, 
                Cleared = true,
                Received = true,
                Saved = true,
                Signed = false,
                SignedByDiocese = false,
                SignedBySecretaryState = true,
                SavedAfterSigningBySecretaryState = false,
            };

            // Act
            await tasksDataClient.UpdateDeedTerminationChurchSupplementalAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.DeedTerminationChurchAgreementNotApplicable);
            Assert.True(existingTaskData.DeedTerminationChurchAgreementCleared);
            Assert.True(existingTaskData.DeedTerminationChurchAgreementReceived);
            Assert.False(existingTaskData.DeedTerminationChurchAgreementSignedOutgoingTrust);
            Assert.False(existingTaskData.DeedTerminationChurchAgreementSignedDiocese);
            Assert.True(existingTaskData.DeedTerminationChurchAgreementSaved); 
            Assert.True(existingTaskData.DeedTerminationChurchAgreementSignedSecretaryState);
            Assert.False(existingTaskData.DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState);
        }
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateDeedTerminationChurchSupplementalAgreementTaskAsync_ShouldNotUpdate_IfNotApplicableIsTrue(
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

            var command = new UpdateDeedTerminationChurchSupplementalAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                NotApplicable = true,
                Cleared = true,
                Received = true,
                Saved = true,
                SavedAfterSigningBySecretaryState = true,
                Signed = true,
                SignedByDiocese = true,
                SignedBySecretaryState = true,
            };

            // Act
            await tasksDataClient.UpdateDeedTerminationChurchSupplementalAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.DeedTerminationChurchAgreementNotApplicable);
            Assert.Null(existingTaskData.DeedTerminationChurchAgreementCleared);
            Assert.Null(existingTaskData.DeedTerminationChurchAgreementReceived);
            Assert.Null(existingTaskData.DeedTerminationChurchAgreementSignedOutgoingTrust);
            Assert.Null(existingTaskData.DeedTerminationChurchAgreementSignedDiocese);
            Assert.Null(existingTaskData.DeedTerminationChurchAgreementSaved);
            Assert.Null(existingTaskData.DeedTerminationChurchAgreementSignedSecretaryState);
            Assert.Null(existingTaskData.DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateDeedTerminationChurchSupplementalAgreementTaskAsync_ShouldFail_WhenTaskDataIdNotMatched(
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

            var command = new UpdateDeedTerminationChurchSupplementalAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = Guid.NewGuid() }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => tasksDataClient.UpdateDeedTerminationChurchSupplementalAgreementTaskAsync(command, default));

            Assert.Contains($"Transfer task data TaskDataId {{ Value = {command.TaskDataId.Value} }} not found.", exception.Message);
        }
    }
}
