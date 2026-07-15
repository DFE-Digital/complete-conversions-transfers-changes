using System.Security.Claims;
using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{
    public class UpdateChildrenCentreTaskTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(OmitCircularReferenceCustomization))]
        public async Task UpdateChildrenCentreTaskAsync_ShouldUpdateSingleSelection(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateChildrenCentreTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ChildrenCentreLocalAuthority = true;

            // Act
            await tasksDataClient.UpdateChildrenCentreTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);

            Assert.Equal(true, existingTaskData.ChildrenCentreLocalAuthority);
        }
        
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(OmitCircularReferenceCustomization))]
        public async Task UpdateChildrenCentreTaskAsync_ShouldUpdateMultiSelection(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateChildrenCentreTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ChildrenCentreLocalAuthority = true;
            command.ChildrenCentreAcademyTrust = true;
            command.ChildrenCentreLandLeaseSharedAgreed = true;

            // Act
            await tasksDataClient.UpdateChildrenCentreTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);

            Assert.Equal(true, existingTaskData.ChildrenCentreLocalAuthority);
            Assert.Equal(true, existingTaskData.ChildrenCentreAcademyTrust);
            Assert.Equal(true, existingTaskData.ChildrenCentreLandLeaseSharedAgreed);
        }
        
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(OmitCircularReferenceCustomization))]
        public async Task UpdateChildrenCentreTaskAsync_ShouldUpdateNotApplicableSelection(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateChildrenCentreTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ChildrenCentreNotApplicable = true;

            // Act
            await tasksDataClient.UpdateChildrenCentreTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);

            Assert.Equal(true, existingTaskData.ChildrenCentreNotApplicable);
        }
        
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(OmitCircularReferenceCustomization))]
        public async Task UpdateChildrenCentreTaskAsync_ShouldUpdateNotApplicable_WhenMultiSelectionContainsNotApplicable(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateChildrenCentreTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ChildrenCentreLocalAuthority = true;
            command.ChildrenCentreFundingPensionReviewed = true;
            command.ChildrenCentreNotApplicable = true;

            // Act
            await tasksDataClient.UpdateChildrenCentreTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);

            Assert.Null(existingTaskData.ChildrenCentreLocalAuthority);
            Assert.Null(existingTaskData.ChildrenCentreFundingPensionReviewed);
            Assert.Equal(true, existingTaskData.ChildrenCentreNotApplicable);
        }
    }
}
