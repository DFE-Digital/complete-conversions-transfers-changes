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
    public class UpdateConfirmSponsoredSupportGrantTaskTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(ConversionTaskDataCustomization))]
        public async Task UpdateConfirmSponsoredSupportGrantTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateConfirmSponsoredSupportGrantTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.SponsoredSupportGrantType = "fast track";
            command.PaymentAmount = true;
            command.PaymentForm = false;
            command.SendInformation = false;
            command.InformTrust = false;

            // Act
            await tasksDataClient.UpdateSponsoredSupportGrantTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Equal("fast track", existingTaskData.SponsoredSupportGrantType);
            Assert.True(existingTaskData.SponsoredSupportGrantPaymentAmount);
            Assert.False(existingTaskData.SponsoredSupportGrantPaymentForm);
            Assert.False(existingTaskData.SponsoredSupportGrantSendInformation);
            Assert.False(existingTaskData.SponsoredSupportGrantInformTrust);
        }
        
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(TransferTaskDataCustomization))]
        public async Task UpdateConfirmSponsoredSupportGrantTaskAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateConfirmSponsoredSupportGrantTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Transfer;
            command.NotApplicable = true;
            command.SponsoredSupportGrantType = "standard";

            // Act
            await tasksDataClient.UpdateSponsoredSupportGrantTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.NotNull(existingTaskData.SponsoredSupportGrantNotApplicable);
            Assert.Null(existingTaskData.SponsoredSupportGrantType);
        }
        
    }
}