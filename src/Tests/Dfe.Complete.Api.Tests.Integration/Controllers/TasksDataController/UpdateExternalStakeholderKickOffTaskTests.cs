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
    public class UpdateExternalStakeholderKickOffTaskTests
    {
        [Theory]
        [CustomAutoData(
           typeof(CustomWebApplicationDbContextFactoryCustomization),
           typeof(TransferTaskDataCustomization),
           typeof(ProjectCustomization))]
        public async Task UpdateExternalStakeholderKickOffTaskAsync_ShouldUpdate_TransferProjectAndTaskData(
           CustomWebApplicationDbContextFactory<Program> factory,
           ITasksDataClient tasksDataClient,
           IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var testUser = await dbContext.Users.FirstAsync();

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
                    Type = Domain.Enums.ProjectType.Transfer
                })
                    .Create<Domain.Entities.Project>();
                project.Urn = establishment.Urn ?? project.Urn;
                return project;
            }).ToList();

            var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
            Assert.NotNull(localAuthority);
            projects.ForEach(x => x.LocalAuthorityId = localAuthority.Id);
            var transferTaskData = fixture.Create<TransferTasksData>();

            await dbContext.Projects.AddRangeAsync(projects);
            await dbContext.SaveChangesAsync();
            var project = projects.First();

            project.TasksDataId = transferTaskData.Id;
            dbContext.TransferTasksData.Add(transferTaskData);
            await dbContext.SaveChangesAsync();

            var testDate = new DateOnly(2023, 10, 5);
            var command = new UpdateExternalStakeholderKickOffTaskCommand
            {
                ProjectId = new ProjectId { Value = project.Id.Value },
                StakeholderKickOffIntroductoryEmails = true,
                LocalAuthorityProforma = true,
                CheckProvisionalDate = true,
                StakeholderKickOffSetupMeeting = true,
                StakeholderKickOffMeeting = true,
                SignificantDate = testDate.ToDateTime(default),
                UserEmail = testUser.Email
            };

            // Act
            await tasksDataClient.UpdateExternalStakeholderKickOffTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == transferTaskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.StakeholderKickOffIntroductoryEmails);
            Assert.True(existingTaskData.StakeholderKickOffSetupMeeting);
            Assert.True(existingTaskData.StakeholderKickOffMeeting);

            var existingProject = await dbContext.Projects
                .Include(x => x.Notes)
                .Include(x => x.SignificantDateHistories)
                    .ThenInclude(h => h.Reasons)
                .SingleOrDefaultAsync(x => x.Id == project.Id);

            Assert.NotNull(existingProject);
            Assert.Single(existingProject.Notes);
            Assert.Single(existingProject.SignificantDateHistories);

            var existingNote = existingProject.Notes.First();
            var existingSignificantDateHistory = existingProject.SignificantDateHistories.First();

            Assert.Single(existingSignificantDateHistory.Reasons);

            var existingSignificantDateHistoryReason = existingSignificantDateHistory.Reasons.First();

            Assert.Equal(testDate, existingProject.SignificantDate);
            Assert.Equal("Transfer date confirmed as part of the External stakeholder kick off task.", existingNote.Body);
            Assert.Equal("stakeholder_kick_off", existingSignificantDateHistoryReason.ReasonType);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization),
            typeof(ProjectCustomization))]
        public async Task UpdateExternalStakeholderKickOffTaskAsync_ShouldUpdate_ConversionProjectAndTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var testUser = await dbContext.Users.FirstAsync();

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
                    Type = Domain.Enums.ProjectType.Conversion
                })
                    .Create<Domain.Entities.Project>();
                project.Urn = establishment.Urn ?? project.Urn;
                return project;
            }).ToList();

            var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
            Assert.NotNull(localAuthority);
            projects.ForEach(x => x.LocalAuthorityId = localAuthority.Id);
            var conversionTaskData = fixture.Create<ConversionTasksData>();

            await dbContext.Projects.AddRangeAsync(projects);
            await dbContext.SaveChangesAsync();
            var project = projects.First();

            project.TasksDataId = conversionTaskData.Id;
            dbContext.ConversionTasksData.Add(conversionTaskData);
            await dbContext.SaveChangesAsync();

            var testDate = new DateOnly(2023, 10, 5);
            var command = new UpdateExternalStakeholderKickOffTaskCommand
            {
                ProjectId = new ProjectId { Value = project.Id.Value },
                StakeholderKickOffIntroductoryEmails = true,
                LocalAuthorityProforma = true,
                CheckProvisionalDate = true,
                StakeholderKickOffSetupMeeting = true,
                StakeholderKickOffMeeting = true,
                SignificantDate = testDate.ToDateTime(default),
                UserEmail = testUser.Email
            };

            // Act
            await tasksDataClient.UpdateExternalStakeholderKickOffTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == conversionTaskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.StakeholderKickOffIntroductoryEmails);
            Assert.True(existingTaskData.StakeholderKickOffCheckProvisionalConversionDate);
            Assert.True(existingTaskData.StakeholderKickOffLocalAuthorityProforma);
            Assert.True(existingTaskData.StakeholderKickOffSetupMeeting);
            Assert.True(existingTaskData.StakeholderKickOffMeeting);

            var existingProject = await dbContext.Projects
                .Include(x => x.Notes)
                .Include(x => x.SignificantDateHistories)
                    .ThenInclude(h => h.Reasons)
                .SingleOrDefaultAsync(x => x.Id == project.Id);

            Assert.NotNull(existingProject);
            Assert.Single(existingProject.Notes);
            Assert.Single(existingProject.SignificantDateHistories);

            var existingNote = existingProject.Notes.First();
            var existingSignificantDateHistory = existingProject.SignificantDateHistories.First();

            Assert.Single(existingSignificantDateHistory.Reasons);

            var existingSignificantDateHistoryReason = existingSignificantDateHistory.Reasons.First();

            Assert.Equal(testDate, existingProject.SignificantDate);
            Assert.Equal("Conversion date confirmed as part of the External stakeholder kick off task.", existingNote.Body);
            Assert.Equal("stakeholder_kick_off", existingSignificantDateHistoryReason.ReasonType);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization))]
        public async Task UpdateExternalStakeholderKickOffTaskAsync_ProjectDoesNotExist_ShouldThrow(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            await dbContext.SaveChangesAsync();

            var conversionTaskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(conversionTaskData);
            await dbContext.SaveChangesAsync();

            var testDate = new DateOnly(2023, 10, 5);
            var command = new UpdateExternalStakeholderKickOffTaskCommand
            {
                ProjectId = new ProjectId { Value = Guid.NewGuid() },
                StakeholderKickOffIntroductoryEmails = true,
                LocalAuthorityProforma = true,
                CheckProvisionalDate = true,
                StakeholderKickOffSetupMeeting = true,
                StakeholderKickOffMeeting = true,
                SignificantDate = testDate.ToDateTime(default),
                UserEmail = "test@example.com"
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                tasksDataClient.UpdateExternalStakeholderKickOffTaskAsync(command, default));
            var validationErrors = exception.Message;
            Assert.NotNull(validationErrors);
            Assert.Contains("Project not found", validationErrors);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(ConversionTaskDataCustomization),
            typeof(ProjectCustomization))]
        public async Task UpdateExternalStakeholderKickOffTaskAsync_UserDoesNotExist_ShouldThrow(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var testUser = await dbContext.Users.FirstAsync();

            var establishments = fixture.Customize(new GiasEstablishmentsCustomization()).CreateMany<Domain.Entities.GiasEstablishment>(1)
                .ToList();

            await dbContext.GiasEstablishments.AddRangeAsync(establishments);
            var projects = establishments.Select(establishment =>
            {
                var project = fixture.Customize(new ProjectCustomization
                {
                    RegionalDeliveryOfficerId = testUser.Id,
                    CaseworkerId = testUser.Id,
                    AssignedToId = testUser.Id
                })
                    .Create<Domain.Entities.Project>();
                project.Urn = establishment.Urn ?? project.Urn;
                return project;
            }).ToList();

            var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
            Assert.NotNull(localAuthority);
            projects.ForEach(x => x.LocalAuthorityId = localAuthority.Id);
            var conversionTaskData = fixture.Create<ConversionTasksData>();

            await dbContext.Projects.AddRangeAsync(projects);
            await dbContext.SaveChangesAsync();
            var project = projects.First();

            project.TasksDataId = conversionTaskData.Id;
            dbContext.ConversionTasksData.Add(conversionTaskData);
            await dbContext.SaveChangesAsync();


            var testDate = new DateOnly(2023, 10, 5);
            var command = new UpdateExternalStakeholderKickOffTaskCommand
            {
                ProjectId = new ProjectId { Value = project.Id.Value },
                StakeholderKickOffIntroductoryEmails = true,
                LocalAuthorityProforma = true,
                CheckProvisionalDate = true,
                StakeholderKickOffSetupMeeting = true,
                StakeholderKickOffMeeting = true,
                SignificantDate = testDate.ToDateTime(default),
                UserEmail = "notfoundtest@example.com"
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                tasksDataClient.UpdateExternalStakeholderKickOffTaskAsync(command, default));
            var validationErrors = exception.Message;
            Assert.NotNull(validationErrors);
            Assert.Contains("User not found", validationErrors);
        }
    }
}