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
    public class UpdateMainContactTaskTests
    {
        [Theory]
        [CustomAutoData(
           typeof(CustomWebApplicationDbContextFactoryCustomization),
           typeof(ProjectCustomization),
           typeof(ContactCustomization))]

        public async Task UpdateMainContactTaskAsync_ShouldUpdate_Project(
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

            var contacts = fixture.CreateMany<Domain.Entities.Contact>(2);
            var oldContact = contacts.ElementAt(0);
            var newContact = contacts.ElementAt(1);

            await dbContext.GiasEstablishments.AddRangeAsync(establishments);
            await dbContext.Contacts.AddRangeAsync(contacts);

            var projects = establishments.Select(establishment =>
            {
                var project = fixture.Customize(new ProjectCustomization
                {
                    RegionalDeliveryOfficerId = testUser.Id,
                    CaseworkerId = testUser.Id,
                    AssignedToId = testUser.Id,
                    MainContactId = oldContact.Id
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

            var command = new UpdateMainContactTaskCommand
            {
                ProjectId = new ProjectId { Value = project.Id.Value },
                MainContactId = new ContactId { Value = newContact.Id.Value },
            };

            // Act
            await tasksDataClient.UpdateMainContactTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingProject = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == project.Id);
            Assert.NotNull(existingProject);
            Assert.Equal(newContact.Id, existingProject.MainContactId);
        }

        [Theory]
        [CustomAutoData(
           typeof(CustomWebApplicationDbContextFactoryCustomization))]

        public async Task UpdateMainContactTaskAsync_Throws_ProjectNotFound(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];
            var dbContext = factory.GetDbContext<CompleteContext>();
            await dbContext.SaveChangesAsync();

            var testProjectId = Guid.NewGuid();
            var command = new UpdateMainContactTaskCommand
            {
                ProjectId = new ProjectId { Value = testProjectId },
                MainContactId = new ContactId { Value = Guid.NewGuid() },
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                tasksDataClient.UpdateMainContactTaskAsync(command, default));
            var validationErrors = exception.Message;
            Assert.NotNull(validationErrors);
            Assert.Contains($"Project {testProjectId} not found", validationErrors);
        }
    }
}