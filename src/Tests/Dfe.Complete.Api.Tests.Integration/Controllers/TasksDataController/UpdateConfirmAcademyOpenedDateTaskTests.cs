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
    public class UpdateConfirmAcademyOpenedDateTaskTests
    {
        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateConfirmAcademyOpenedDateTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            taskData.ConfirmDateAcademyOpenedDateOpened = null;
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();

            var testDate = new DateOnly(2023, 10, 5);
            var command = new UpdateConfirmAcademyOpenedDateTaskCommand
            {
                TaskDataId = new TaskDataId { Value = taskData.Id.Value },
                AcademyOpenedDate = testDate.ToDateTime(default),
            };

            // Act
            await tasksDataClient.UpdateConfirmAcademyOpenedDateTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Equal(testDate, existingTaskData.ConfirmDateAcademyOpenedDateOpened);
        }
    }
}