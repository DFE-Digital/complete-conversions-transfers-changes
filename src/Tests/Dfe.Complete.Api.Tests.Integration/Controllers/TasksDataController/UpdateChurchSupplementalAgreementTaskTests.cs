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
    public class UpdateChurchSupplementalAgreementTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateChurchSupplementalAgreementTaskAsync_ShouldUpdate_TransferTaskData(
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

            var command = new UpdateChurchSupplementalAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Transfer,
                Cleared = true,
                Received = true,
                Saved = true,
                SentOrSaved = true,
                Signed = true,
                SignedByDiocese = true,
                SignedBySecretaryState = true,
            };

            // Act
            await tasksDataClient.UpdateChurchSupplementalAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ChurchSupplementalAgreementCleared);
            Assert.True(existingTaskData.ChurchSupplementalAgreementReceived);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSavedAfterSigningBySecretaryState);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedIncomingTrust);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedDiocese);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedSecretaryState);
            Assert.Null(existingTaskData.ChurchSupplementalAgreementNotApplicable);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateChurchSupplementalAgreementTaskAsync_ShouldUpdate_ConversionTaskData(
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

            var command = new UpdateChurchSupplementalAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Conversion,
                Received = true,
                Cleared = true,
                Signed = true,
                SignedByDiocese = true,
                Saved = true,
                SignedBySecretaryState = true,
                SentOrSaved = true,
            };

            // Act
            await tasksDataClient.UpdateChurchSupplementalAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ChurchSupplementalAgreementReceived);
            Assert.True(existingTaskData.ChurchSupplementalAgreementCleared);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSigned);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSaved);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedDiocese);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedSecretaryState);
            Assert.Null(existingTaskData.ChurchSupplementalAgreementNotApplicable);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateChurchSupplementalAgreementTaskAsync_ShouldUpdateNotApplicableOnly(
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

            var command = new UpdateChurchSupplementalAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Conversion,
                Received = true,
                Cleared = true,
                Signed = true,
                SignedByDiocese = true,
                Saved = true,
                SignedBySecretaryState = true,
                SentOrSaved = true,
                NotApplicable = true
            };

            // Act
            await tasksDataClient.UpdateChurchSupplementalAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.ChurchSupplementalAgreementReceived);
            Assert.Null(existingTaskData.ChurchSupplementalAgreementCleared);
            Assert.Null(existingTaskData.ChurchSupplementalAgreementSigned);
            Assert.Null(existingTaskData.ChurchSupplementalAgreementSignedDiocese);
            Assert.Null(existingTaskData.ChurchSupplementalAgreementSaved);
            Assert.Null(existingTaskData.ChurchSupplementalAgreementSignedSecretaryState);
            Assert.Null(existingTaskData.ChurchSupplementalAgreementSent);
            Assert.True(existingTaskData.ChurchSupplementalAgreementNotApplicable);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateChurchSupplementalAgreementTaskAsync_ShouldFail_WhenProjectTypeOmitted(
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

            var command = new UpdateChurchSupplementalAgreementTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<CompleteApiException>(() => tasksDataClient.UpdateChurchSupplementalAgreementTaskAsync(command, default));

            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

            var validationErrors = exception.Response;
            Assert.NotNull(validationErrors);
            Assert.Contains("The ProjectType field is required.", validationErrors);
        }
    }
}