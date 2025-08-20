using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; 

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController
{
    public class TaskDataControllerTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetTransferTasksDataByIdAsync_ShouldReturn_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var task = fixture.Build<TransferTasksData>()
                .With(tt => tt.Id, new Domain.ValueObjects.TaskDataId(Guid.NewGuid()))
                .Create();

            await dbContext.TransferTasksData.AddAsync(task);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await tasksDataClient.GetTransferTasksDataByIdAsync(task.Id.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id.Value, result.Id!.Value);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateHandoverWithDeliveryOfficerTaskDataByProjectIdAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateHandoverWithDeliveryOfficerTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>(); 

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData); 

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Conversion;
            command.NotApplicable = true;

            // Act
            await tasksDataClient.UpdateHandoverWithDeliveryOfficerTaskDataByTaskDataIdAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id); 
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.HandoverNotApplicable);
            Assert.Null(existingTaskData.HandoverReview);
            Assert.Null(existingTaskData.HandoverMeeting);
            Assert.Null(existingTaskData.HandoverNotes);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateHandoverWithDeliveryOfficerTaskDataByProjectIdAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateHandoverWithDeliveryOfficerTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>(); 

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData); 

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Transfer;
            command.HandoverMeetings = true;
            command.HandoverReview = false;
            command.HandoverNotes = true; 

            // Act
            await tasksDataClient.UpdateHandoverWithDeliveryOfficerTaskDataByTaskDataIdAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.HandoverNotApplicable);
            Assert.False(existingTaskData.HandoverReview);
            Assert.True(existingTaskData.HandoverMeeting);
            Assert.True(existingTaskData.HandoverNotes);
        } 
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetConversionTasksDataByIdAsync_ShouldReturn_A_TaskDaa(
        CustomWebApplicationDbContextFactory<Program> factory,
        ITasksDataClient tasksDatClient,
        IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var task = fixture.Build<ConversionTasksData>()
                .With(tt => tt.Id, new Domain.ValueObjects.TaskDataId(Guid.NewGuid()))
                .Create();

            await dbContext.ConversionTasksData.AddAsync(task);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await tasksDatClient.GetConversionTasksDataByIdAsync(task.Id.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id.Value, result.Id!.Value);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateArticleOfAssociationTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateArticleOfAssociationTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Conversion;
            command.NotApplicable = true;

            // Act
            await tasksDataClient.UpdateArticleOfAssociationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ArticlesOfAssociationNotApplicable);
            Assert.Null(existingTaskData.ArticlesOfAssociationCleared);
            Assert.Null(existingTaskData.ArticlesOfAssociationReceived);
            Assert.Null(existingTaskData.ArticlesOfAssociationSaved); 
            Assert.Null(existingTaskData.ArticlesOfAssociationSent); 
        }
    }
}
