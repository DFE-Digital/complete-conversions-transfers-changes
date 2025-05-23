using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers
{
    public class ServiceSupportControllerTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task CreateLocalAuthorityAsyncShouldCrateLocalAuthorityAndContact(
        CustomWebApplicationDbContextFactory<Program> factory,
        IServiceSupportClient serviceSupportClient,
        IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole}
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();
            var command = new CreateLocalAuthorityCommand()
            {
                Id = new LocalAuthorityId() { Value = Guid.NewGuid() },
                Code = fixture.Create<int>().ToString(),
                Name = fixture.Create<string>(),
                Address1 = fixture.Create<string>(),
                Address2 = fixture.Create<string>(),
                AddressTown = fixture.Create<string>(),
                AddressCounty = fixture.Create<string>(),
                AddressPostcode = fixture.Create<string>(),
                ContactName = fixture.Create<string>(),
                Title = fixture.Create<string>(),
                ContactId = new ContactId { Value = Guid.NewGuid() }
            };
            var localAuthorityId = await serviceSupportClient.CreateLocalAuthorityAsync(command, CancellationToken.None);

            var newLocalAuthority = await dbContext.LocalAuthorities.SingleOrDefaultAsync(x => x.Id == new Domain.ValueObjects.LocalAuthorityId(localAuthorityId));

            Assert.NotNull(newLocalAuthority);

            var newContact = await dbContext.Contacts.SingleOrDefaultAsync(x => x.Id == new Domain.ValueObjects.ContactId(command.ContactId.Value.Value));

            Assert.NotNull(newContact);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task RemoveLocalAuthorityAsyncShouldRemoveLocalAuthorityAndContact(
        CustomWebApplicationDbContextFactory<Program> factory,
        IServiceSupportClient serviceSupportClient,
        IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.DeleteRole, ApiRoles.UpdateRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();

            var localAuthority = fixture.Customize(new LocalAuthorityCustomization
            {
                LocalAuthorityCode = fixture.Create<int>().ToString(),

            }).Create<Domain.Entities.LocalAuthority>();
            await dbContext.LocalAuthorities.AddAsync(localAuthority);

            var contact = Domain.Entities.Contact.CreateLocalAuthorityContact(new Domain.ValueObjects.ContactId(Guid.NewGuid()),
                "title", "name", null, null, localAuthority.Id, DateTime.Now); 
            await dbContext.Contacts.AddAsync(contact);

            await dbContext.SaveChangesAsync();

            var existingLocalAuthoritybefore = await dbContext.LocalAuthorities.SingleAsync(x => x.Code == localAuthority.Code);

            Assert.NotNull(existingLocalAuthoritybefore);

            var existingContactbefore = await dbContext.Contacts.SingleAsync(x => x.Id == contact.Id);

            Assert.NotNull(existingContactbefore);

            await serviceSupportClient.RemoveLocalAuthorityAsync(new DeleteLocalAuthorityCommand
            {
                Id = new LocalAuthorityId() { Value = localAuthority.Id.Value },
                ContactId = new ContactId() { Value = contact.Id.Value }
            }, CancellationToken.None);

            var existinglocalAuthority= await dbContext.LocalAuthorities.SingleOrDefaultAsync(x => x.Id == localAuthority.Id);

            Assert.Null(existinglocalAuthority);

            var existingContact = await dbContext.Contacts.SingleOrDefaultAsync(x => x.Id == contact.Id);

            Assert.Null(existingContact);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task RemoveLocalAuthorityAsyncShouldRemoveLocalAuthorityWithNoContact(
        CustomWebApplicationDbContextFactory<Program> factory,
        IServiceSupportClient serviceSupportClient,
        IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.DeleteRole, ApiRoles.UpdateRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();

            var localAuthority = fixture.Customize(new LocalAuthorityCustomization
            {
                LocalAuthorityCode = fixture.Create<int>().ToString(),

            }).Create<Domain.Entities.LocalAuthority>();
            dbContext.LocalAuthorities.Add(localAuthority);
            await dbContext.SaveChangesAsync();

            var existingLocalAuthoritybefore = await dbContext.LocalAuthorities.SingleAsync(x => x.Code == localAuthority.Code);

            Assert.NotNull(existingLocalAuthoritybefore);

            await serviceSupportClient.RemoveLocalAuthorityAsync(new DeleteLocalAuthorityCommand { Id = new LocalAuthorityId() { Value = localAuthority.Id.Value } }, CancellationToken.None);

            var existinglocalAuthority = await dbContext.LocalAuthorities.SingleOrDefaultAsync(x => x.Id == localAuthority.Id);

            Assert.Null(existinglocalAuthority);
        }
    }
}
