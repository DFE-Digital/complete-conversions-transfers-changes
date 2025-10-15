using AutoFixture;
using AutoFixture.Xunit2;
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
    public class UpdateMasterFundingAgreementTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateMasterFundingAgreementTaskAsync_ShouldUpdate_TransferTaskData(
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

            var command = new UpdateMasterFundingAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Transfer,
                Received = true,
                Cleared = true,
                Signed = true,
                Saved = true,
                Sent = true,
                SignedSecretaryState = true
            };

            // Act
            await tasksDataClient.UpdateMasterFundingAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.MasterFundingAgreementReceived);
            Assert.True(existingTaskData.MasterFundingAgreementCleared);
            Assert.True(existingTaskData.MasterFundingAgreementSaved);
            Assert.True(existingTaskData.MasterFundingAgreementSignedSecretaryState);
            Assert.True(existingTaskData.MasterFundingAgreementSigned);
            Assert.Null(existingTaskData.MasterFundingAgreementNotApplicable);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateMasterFundingAgreementTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            [Frozen]
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            var command = new UpdateMasterFundingAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Conversion,
                Received = true,
                Cleared = true,
                Signed = true,
                Saved = true,
                Sent = true,
                SignedSecretaryState = true
            };

            // Act
            await tasksDataClient.UpdateMasterFundingAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.MasterFundingAgreementReceived);
            Assert.True(existingTaskData.MasterFundingAgreementCleared);
            Assert.True(existingTaskData.MasterFundingAgreementSaved);
            Assert.True(existingTaskData.MasterFundingAgreementSent);
            Assert.True(existingTaskData.MasterFundingAgreementSignedSecretaryState);
            Assert.True(existingTaskData.MasterFundingAgreementSigned);
            Assert.Null(existingTaskData.MasterFundingAgreementNotApplicable);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateMasterFundingAgreementTaskAsync_ShouldUpdateNotApplicableOnly(
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

            var command = new UpdateMasterFundingAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Conversion,
                Received = true,
                Cleared = true,
                Saved = true,
                Sent = true,
                Signed = true,
                NotApplicable = true
            };

            // Act
            await tasksDataClient.UpdateMasterFundingAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.MasterFundingAgreementReceived);
            Assert.Null(existingTaskData.MasterFundingAgreementCleared);
            Assert.Null(existingTaskData.MasterFundingAgreementSaved);
            Assert.Null(existingTaskData.MasterFundingAgreementSent);
            Assert.Null(existingTaskData.MasterFundingAgreementSignedSecretaryState);
            Assert.Null(existingTaskData.MasterFundingAgreementSigned);
            Assert.True(existingTaskData.MasterFundingAgreementNotApplicable);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateMasterFundingAgreementTaskAsync_ShouldFail_WhenProjectTypeOmitted(
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

            var command = new UpdateMasterFundingAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<CompleteApiException>(() => tasksDataClient.UpdateMasterFundingAgreementTaskAsync(command, default));

            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

            var validationErrors = exception.Response;
            Assert.NotNull(validationErrors);
            Assert.Contains("The ProjectType field is required.", validationErrors);
        }
    }
}