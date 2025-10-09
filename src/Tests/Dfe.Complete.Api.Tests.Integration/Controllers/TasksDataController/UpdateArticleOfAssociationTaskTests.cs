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
    public class UpdateArticleOfAssociationTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateArticleOfAssociationTaskAsync_ShouldUpdate_TransferTaskData(
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

            var command = new UpdateArticleOfAssociationTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Transfer,
                Received = true,
                Cleared = true,
                Saved = true,
                Sent = true,
                Signed = true,
            };

            // Act
            await tasksDataClient.UpdateArticleOfAssociationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ArticlesOfAssociationReceived);
            Assert.True(existingTaskData.ArticlesOfAssociationCleared);
            Assert.True(existingTaskData.ArticlesOfAssociationSaved);
            Assert.True(existingTaskData.ArticlesOfAssociationSent);
            Assert.True(existingTaskData.ArticlesOfAssociationSigned);
            Assert.Null(existingTaskData.ArticlesOfAssociationNotApplicable);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateArticleOfAssociationTaskAsync_ShouldUpdate_ConversionTaskData(
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

            var command = new UpdateArticleOfAssociationTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ProjectType = ProjectType.Conversion,
                Received = true,
                Cleared = true,
                Saved = true,
                Sent = true,
                Signed = true,
            };

            // Act
            await tasksDataClient.UpdateArticleOfAssociationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ArticlesOfAssociationReceived);
            Assert.True(existingTaskData.ArticlesOfAssociationCleared);
            Assert.True(existingTaskData.ArticlesOfAssociationSaved);
            Assert.True(existingTaskData.ArticlesOfAssociationSent);
            Assert.True(existingTaskData.ArticlesOfAssociationSigned);
            Assert.Null(existingTaskData.ArticlesOfAssociationNotApplicable);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateArticleOfAssociationTaskAsync_ShouldUpdateNotApplicableOnly(
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

            var command = new UpdateArticleOfAssociationTaskCommand
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
            await tasksDataClient.UpdateArticleOfAssociationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.ArticlesOfAssociationReceived);
            Assert.Null(existingTaskData.ArticlesOfAssociationCleared);
            Assert.Null(existingTaskData.ArticlesOfAssociationSaved);
            Assert.Null(existingTaskData.ArticlesOfAssociationSent);
            Assert.Null(existingTaskData.ArticlesOfAssociationSigned);
            Assert.True(existingTaskData.ArticlesOfAssociationNotApplicable);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateArticleOfAssociationTaskAsync_ShouldFail_WhenProjectTypeOmitted(
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

            var command = new UpdateArticleOfAssociationTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value }
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<CompleteApiException>(() => tasksDataClient.UpdateArticleOfAssociationTaskAsync(command, default));

            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

            var validationErrors = exception.Response;
            Assert.NotNull(validationErrors);
            Assert.Contains("The ProjectType field is required.", validationErrors);
        }
    }
}