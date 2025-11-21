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
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.TasksDataController
{
    public class UpdateConfirmHeadTeacherContactTaskTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(OmitCircularReferenceCustomization))]
        public async Task UpdateConfirmHeadTeacherContactTaskAsync_ShouldUpdate_KeyContact(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateHeadTeacherCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var keycontact = fixture.Create<KeyContact>();
            var contact = fixture.Create<Domain.Entities.Contact>();
            dbContext.KeyContacts.Add(keycontact);

            await dbContext.SaveChangesAsync();

            command.HeadTeacherId = new ContactId { Value = contact.Id.Value };
            command.KeyContactId = new KeyContactId { Value = keycontact.Id.Value };

            // Act
            await tasksDataClient.UpdateConfirmHeadTeacherContactTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingKeyContactData = await dbContext.KeyContacts.SingleOrDefaultAsync(x => x.Id == keycontact.Id);
            Assert.NotNull(existingKeyContactData);
            Assert.Equal(contact.Id.Value, existingKeyContactData.HeadteacherId?.Value);
        }

    }
}