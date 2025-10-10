using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{
    public class UpdateConfirmProposedCapacityOfTheAcademyTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateConfirmProposedCapacityOfTheAcademyTaskAsync_ShouldUpdate_ConversionTaskData(
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

            var command = new UpdateConfirmProposedCapacityOfTheAcademyTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ReceptionToSixYears = "100",
                SevenToElevenYears = "200",
                TwelveOrAboveYears = "300",
            };

            // Act
            await tasksDataClient.UpdateConfirmProposedCapacityOfTheAcademyTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Equal("100", existingTaskData.ProposedCapacityOfTheAcademyReceptionToSixYears);
            Assert.Equal("200", existingTaskData.ProposedCapacityOfTheAcademySevenToElevenYears);
            Assert.Equal("300", existingTaskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears);
            Assert.Null(existingTaskData.ProposedCapacityOfTheAcademyNotApplicable);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateConfirmProposedCapacityOfTheAcademyTaskAsync_ShouldUpdateNotApplicableOnly(
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

            var command = new UpdateConfirmProposedCapacityOfTheAcademyTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                ReceptionToSixYears = "100",
                SevenToElevenYears = "200",
                TwelveOrAboveYears = "300",
                NotApplicable = true
            };

            // Act
            await tasksDataClient.UpdateConfirmProposedCapacityOfTheAcademyTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Null(existingTaskData.ProposedCapacityOfTheAcademyReceptionToSixYears);
            Assert.Null(existingTaskData.ProposedCapacityOfTheAcademySevenToElevenYears);
            Assert.Null(existingTaskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears);
            Assert.True(existingTaskData.ProposedCapacityOfTheAcademyNotApplicable);
        }
    }
}