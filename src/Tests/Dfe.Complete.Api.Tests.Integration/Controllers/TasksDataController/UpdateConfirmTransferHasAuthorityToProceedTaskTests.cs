using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{
    public class UpdateConfirmTransferHasAuthorityToProceedTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
           typeof(ProjectCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateConfirmTransferHasAuthorityToProceedTaskAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();
            var testUser = await dbContext.Users.FirstAsync();
            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            var establishments = fixture.Customize(new GiasEstablishmentsCustomization()).CreateMany<Domain.Entities.GiasEstablishment>(1)
                .ToList();

            await dbContext.GiasEstablishments.AddRangeAsync(establishments);
            var projects = establishments.Select(establishment =>
            {
                var project = fixture.Customize(new ProjectCustomization
                {
                    RegionalDeliveryOfficerId = testUser.Id,
                    CaseworkerId = testUser.Id,
                    AssignedToId = testUser.Id,
                    TasksDataId = taskData.Id
                })
                    .Create<Domain.Entities.Project>();
                project.Urn = establishment.Urn ?? project.Urn;
                return project;
            }).ToList();

            var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
            Assert.NotNull(localAuthority);
            projects.ForEach(x => x.LocalAuthorityId = localAuthority.Id);

            await dbContext.Projects.AddRangeAsync(projects);
            await dbContext.SaveChangesAsync();
            var projectId = projects.First().Id;


            await dbContext.SaveChangesAsync();

            var command = new UpdateConfirmTransferHasAuthorityToProceedTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                AnyInformationChanged = true,
                BaselineSheetApproved = true,
                ConfirmToProceed = true,
            };

            // Act
            await tasksDataClient.UpdateConfirmTransferHasAuthorityToProceedTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ConditionsMetBaselineSheetApproved);
            Assert.True(existingTaskData.ConditionsMetCheckAnyInformationChanged);

            var existingProject = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == projectId);
            Assert.NotNull(existingProject);
            Assert.True(existingProject.AllConditionsMet);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
           typeof(ProjectCustomization),
            typeof(TransferTaskDataCustomization))]
        public async Task UpdateConfirmTransferHasAuthorityToProceedTaskAsync_ProjectDoesNotExist_ShouldThrow(
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

            var command = new UpdateConfirmTransferHasAuthorityToProceedTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                AnyInformationChanged = true,
                BaselineSheetApproved = true,
                ConfirmToProceed = true,
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => tasksDataClient.UpdateConfirmTransferHasAuthorityToProceedTaskAsync(command, default));

            var validationErrors = exception.Message;
            Assert.NotNull(validationErrors);
            Assert.Contains($"Project with task data id {taskData.Id.Value} not found.", validationErrors);

        }
    }
}