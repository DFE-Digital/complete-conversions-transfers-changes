using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{
    public class UpdateConfirmAllConditionsMetTaskTests
    {
        [Theory]
        [CustomAutoData(
           typeof(CustomWebApplicationDbContextFactoryCustomization),
           typeof(ProjectCustomization))]
        public async Task UpdateConfirmAllConditionsMetTaskAsync_ShouldUpdate_Project(
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

            await dbContext.Projects.AddRangeAsync(projects);
            await dbContext.SaveChangesAsync();
            var projectId = projects.First().Id;

            var command = new UpdateConfirmAllConditionsMetTaskCommand
            {
                ProjectId = new ProjectId { Value = projectId.Value },
                Confirm = true,
            };

            // Act
            await tasksDataClient.UpdateConfirmAllConditionsMetTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingProject = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == projectId);
            Assert.NotNull(existingProject);
            Assert.True(existingProject.AllConditionsMet);
        }
    }
}