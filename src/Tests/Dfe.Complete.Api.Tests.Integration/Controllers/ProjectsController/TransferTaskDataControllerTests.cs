using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations; 
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController
{
    public class TransferTaskDataControllerTests
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
            UpdateHandoverWithDeliveryOfficerCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var testUser = await dbContext.Users.FirstAsync();
            var establishment = fixture.Create<Domain.Entities.GiasEstablishment>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            dbContext.GiasEstablishments.Add(establishment);
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id,
                TasksDataId = taskData.Id,
                TasksDataType = Domain.Enums.TaskType.Conversion,
                Type = Domain.Enums.ProjectType.Conversion
            }).Create<Domain.Entities.Project>();
            project.Urn = establishment.Urn ?? project.Urn;

            var localAuthority = await dbContext.LocalAuthorities.FirstOrDefaultAsync();
            Assert.NotNull(localAuthority);
            project.LocalAuthorityId = localAuthority.Id;

            dbContext.Projects.Add(project);

            await dbContext.SaveChangesAsync();
            command.ProjectId = new ProjectId { Value = project.Id.Value };
            command.NotApplicable = true;

            // Act
            await tasksDataClient.UpdateHandoverWithDeliveryOfficerTaskDataByProjectIdAsync(command, default);

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
            UpdateHandoverWithDeliveryOfficerCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var testUser = await dbContext.Users.FirstAsync();
            var establishment = fixture.Create<Domain.Entities.GiasEstablishment>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            dbContext.GiasEstablishments.Add(establishment);
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id,
                TasksDataId = taskData.Id,
                TasksDataType = Domain.Enums.TaskType.Transfer,
                Type = Domain.Enums.ProjectType.Transfer
            }).Create<Domain.Entities.Project>();
            project.Urn = establishment.Urn ?? project.Urn;

            var localAuthority = await dbContext.LocalAuthorities.FirstOrDefaultAsync();
            Assert.NotNull(localAuthority);
            project.LocalAuthorityId = localAuthority.Id; 

            dbContext.Projects.Add(project);

            await dbContext.SaveChangesAsync();
            command.ProjectId = new ProjectId { Value = project.Id.Value };
            command.HandoverMeetings = true;
            command.HandoverReview = false;
            command.HandoverNotes = true; 

            // Act
            await tasksDataClient.UpdateHandoverWithDeliveryOfficerTaskDataByProjectIdAsync(command, default);

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
        public async Task GetTaskDataByProjectIdAsync_ShouldReturn_TransferTaskData(
           CustomWebApplicationDbContextFactory<Program> factory,
           ITasksDataClient tasksDataClient,
           IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var testUser = await dbContext.Users.FirstAsync();
            var establishment = fixture.Create<Domain.Entities.GiasEstablishment>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            dbContext.GiasEstablishments.Add(establishment);
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id,
                TasksDataId = taskData.Id,
                TasksDataType = Domain.Enums.TaskType.Transfer,
                Type = Domain.Enums.ProjectType.Transfer
            }).Create<Domain.Entities.Project>();
            project.Urn = establishment.Urn ?? project.Urn;

            var localAuthority = await dbContext.LocalAuthorities.FirstOrDefaultAsync();
            Assert.NotNull(localAuthority);
            project.LocalAuthorityId = localAuthority.Id;

            dbContext.Projects.Add(project);

            await dbContext.SaveChangesAsync(); 

            // Act
            var result = await tasksDataClient.GetTaskDataByProjectIdAsync(project.Id.Value, default);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskDataModel>(result); 
            Assert.Equal(taskData.Id.Value, result.TaskDataId!.Value);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetTaskDataByProjectIdAsync_ShouldReturn_ConversionTaskData(
          CustomWebApplicationDbContextFactory<Program> factory,
          ITasksDataClient tasksDataClient,
          IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var testUser = await dbContext.Users.FirstAsync();
            var establishment = fixture.Create<Domain.Entities.GiasEstablishment>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            dbContext.GiasEstablishments.Add(establishment);
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id,
                TasksDataId = taskData.Id,
                TasksDataType = Domain.Enums.TaskType.Conversion,
                Type = Domain.Enums.ProjectType.Conversion
            }).Create<Domain.Entities.Project>();
            project.Urn = establishment.Urn ?? project.Urn;

            var localAuthority = await dbContext.LocalAuthorities.FirstOrDefaultAsync();
            Assert.NotNull(localAuthority);
            project.LocalAuthorityId = localAuthority.Id;

            dbContext.Projects.Add(project);

            await dbContext.SaveChangesAsync();

            // Act
            var result = await tasksDataClient.GetTaskDataByProjectIdAsync(project.Id.Value, default);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskDataModel>(result);
            Assert.Equal(taskData.Id.Value, result.TaskDataId!.Value);
        }
    }
}
