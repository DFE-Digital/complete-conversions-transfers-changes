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
        public async Task CreateLocalAuthorityAsyncShouldCreateLocalAuthorityAndContact(
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
        public async Task CreateLocalAuthorityAsyncShouldCreateLocalAuthorityWithoutContact(
        CustomWebApplicationDbContextFactory<Program> factory,
        IServiceSupportClient serviceSupportClient,
        IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole }
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
                AddressPostcode = fixture.Create<string>()
            };

            var localAuthorityId = await serviceSupportClient.CreateLocalAuthorityAsync(command, CancellationToken.None);

            var newLocalAuthority = await dbContext.LocalAuthorities.SingleOrDefaultAsync(x => x.Id == new Domain.ValueObjects.LocalAuthorityId(localAuthorityId));

            Assert.NotNull(newLocalAuthority);

            var newContact = await dbContext.Contacts.SingleOrDefaultAsync();

            Assert.Null(newContact);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task CreateLocalAuthorityAsyncShouldNotCreateLocalAuthorityIfAlreadyExists(
        CustomWebApplicationDbContextFactory<Program> factory,
        IServiceSupportClient serviceSupportClient,
        IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole}
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();
            var command = new CreateLocalAuthorityCommand
            {
                Id = new LocalAuthorityId() { Value = Guid.NewGuid() },
                Code = fixture.Create<int>().ToString(),
                Name = fixture.Create<string>(),
                Address1 = fixture.Create<string>(),
                Address2 = fixture.Create<string>(),
                AddressTown = fixture.Create<string>(),
                AddressCounty = fixture.Create<string>(),
                AddressPostcode = fixture.Create<string>()
            };

            var localAuthority = fixture.Customize(new LocalAuthorityCustomization
            {
                LocalAuthorityCode = command.Code,
                LocalAuthorityName = command.Name

            }).Create<Domain.Entities.LocalAuthority>();
            await dbContext.LocalAuthorities.AddAsync(localAuthority); 

            await dbContext.SaveChangesAsync();  

            await Assert.ThrowsAsync<CompleteApiException>(() => serviceSupportClient.CreateLocalAuthorityAsync(command, CancellationToken.None)); 
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateLocalAuthorityDetailsAsyncShouldUpdateLocalAuthorityAndContact(
           CustomWebApplicationDbContextFactory<Program> factory,
           IServiceSupportClient serviceSupportClient,
           IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.UpdateRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList(); 

            var dbContext = factory.GetDbContext<CompleteContext>();

            var localAuthority = fixture.Customize(new LocalAuthorityCustomization
            {
                LocalAuthorityCode = fixture.Create<int>().ToString(),
                LocalAuthorityName = fixture.Create<string>()

            }).Create<Domain.Entities.LocalAuthority>();
            await dbContext.LocalAuthorities.AddAsync(localAuthority);

            var contact = Domain.Entities.Contact.CreateLocalAuthorityContact(new Domain.ValueObjects.ContactId(Guid.NewGuid()),
                "title", "name", null, null, localAuthority.Id, DateTime.Now);
            await dbContext.Contacts.AddAsync(contact);

            await dbContext.SaveChangesAsync();

            var command = new UpdateLocalAuthorityCommand()
            {
                Id = new LocalAuthorityId { Value = localAuthority.Id.Value },
                Code = fixture.Create<int>().ToString(),
                Address1 = fixture.Create<string>(),
                Address2 = fixture.Create<string>(),
                Address3 = fixture.Create<string>(),
                AddressTown = fixture.Create<string>(),
                AddressCounty = fixture.Create<string>(),
                AddressPostcode = fixture.Create<string>(),
                ContactName = fixture.Create<string>(),
                Title = fixture.Create<string>(),
                ContactId = new ContactId { Value = contact.Id.Value },
                Email = fixture.Create<string>(),
                Phone = fixture.Create<string>()
            };

            await serviceSupportClient.UpdateLocalAuthorityDetailsAsync(command, CancellationToken.None);

            var existinglocalAuthority = await dbContext.LocalAuthorities.SingleOrDefaultAsync(x => x.Id == localAuthority.Id);

            Assert.NotNull(existinglocalAuthority);

            var existingContact = await dbContext.Contacts.SingleOrDefaultAsync(x => x.Id == contact.Id);
            Assert.NotNull(existingContact);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateLocalAuthorityDetailsAsyncShouldUpdateLocalAuthorityAndAddContact(
          CustomWebApplicationDbContextFactory<Program> factory,
          IServiceSupportClient serviceSupportClient,
          IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.UpdateRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();
            dbContext.LocalAuthorities.RemoveRange(dbContext.LocalAuthorities);
            var localAuthority = fixture.Customize(new LocalAuthorityCustomization
            {
                LocalAuthorityCode = fixture.Create<int>().ToString(),
                LocalAuthorityName = fixture.Create<string>()

            }).Create<Domain.Entities.LocalAuthority>();
            await dbContext.LocalAuthorities.AddAsync(localAuthority); 

            await dbContext.SaveChangesAsync();

            var command = new UpdateLocalAuthorityCommand()
            {
                Id = new LocalAuthorityId { Value = localAuthority.Id.Value },
                Code = fixture.Create<int>().ToString(),
                Address1 = fixture.Create<string>(),
                Address2 = fixture.Create<string>(),
                Address3 = fixture.Create<string>(),
                AddressTown = fixture.Create<string>(),
                AddressCounty = fixture.Create<string>(),
                AddressPostcode = fixture.Create<string>(),
                ContactName = fixture.Create<string>(),
                Title = fixture.Create<string>(),
                ContactId = new ContactId { Value = Guid.NewGuid() },
                Email = fixture.Create<string>(),
                Phone = fixture.Create<string>()
            };

            await serviceSupportClient.UpdateLocalAuthorityDetailsAsync(command, CancellationToken.None);

            var existinglocalAuthority = await dbContext.LocalAuthorities.SingleOrDefaultAsync(x => x.Id == new Domain.ValueObjects.LocalAuthorityId(command.Id.Value!.Value));

            Assert.NotNull(existinglocalAuthority);

            var newContact = await dbContext.Contacts.SingleOrDefaultAsync(x => x.Name == command.ContactName);
            Assert.NotNull(newContact);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateLocalAuthorityDetailsAsyncShouldThrowExpceptionIfLocalAuthorityDoesNotExist(
          CustomWebApplicationDbContextFactory<Program> factory,
          IServiceSupportClient serviceSupportClient,
          IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.UpdateRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();
            dbContext.LocalAuthorities.RemoveRange(dbContext.LocalAuthorities);
            var command = new UpdateLocalAuthorityCommand()
            {
                Id = new LocalAuthorityId { Value = Guid.NewGuid() },
                Code = fixture.Create<int>().ToString(),
                Address1 = fixture.Create<string>(),
                Address2 = fixture.Create<string>(),
                Address3 = fixture.Create<string>(),
                AddressTown = fixture.Create<string>(),
                AddressCounty = fixture.Create<string>(),
                AddressPostcode = fixture.Create<string>(),
            };

            await serviceSupportClient.UpdateLocalAuthorityDetailsAsync(command, CancellationToken.None);

            var localAuthority = await dbContext.LocalAuthorities.SingleOrDefaultAsync(x => x.Id == new Domain.ValueObjects.LocalAuthorityId(command.Id.Value!.Value));

            Assert.Null(localAuthority);
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

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task ListAllLocalAuthoritiesAsyncShouldReturnListofLocalAuthorities(
           CustomWebApplicationDbContextFactory<Program> factory,
           IServiceSupportClient serviceSupportClient,
           IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();
            dbContext.LocalAuthorities.RemoveRange(dbContext.LocalAuthorities);
            var localAuthorities = fixture.Customize(new LocalAuthorityCustomization
            {
                LocalAuthorityCode = fixture.Create<int>().ToString(),
                LocalAuthorityName = fixture.Create<string>()

            }).CreateMany<Domain.Entities.LocalAuthority>(10);
            await dbContext.LocalAuthorities.AddRangeAsync(localAuthorities);
            
            await dbContext.SaveChangesAsync(); 

            var response = await serviceSupportClient.ListAllLocalAuthoritiesAsync(0, 10, CancellationToken.None);

            Assert.NotNull(response);  
            Assert.Equal(10, response.Count);
            foreach (var result in response)
            {
                var loalAuthority = localAuthorities.FirstOrDefault(x => x.Name == result.Name);
                Assert.NotNull(loalAuthority); 
            }
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetLocalAuthorityDetailsAsyncShouldReturnDetails(
           CustomWebApplicationDbContextFactory<Program> factory,
           IServiceSupportClient serviceSupportClient,
           IFixture fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();

            var localAuthority = fixture.Customize(new LocalAuthorityCustomization
            {
                LocalAuthorityCode = fixture.Create<int>().ToString(),
                LocalAuthorityName = fixture.Create<string>()

            }).Create<Domain.Entities.LocalAuthority>();
            await dbContext.LocalAuthorities.AddAsync(localAuthority);
           
            var contact = Domain.Entities.Contact.CreateLocalAuthorityContact(new Domain.ValueObjects.ContactId(Guid.NewGuid()),
                fixture.Create<string>(), fixture.Create<string>(), null, null, localAuthority.Id, DateTime.Now);
            await dbContext.Contacts.AddAsync(contact);
            
            await dbContext.SaveChangesAsync();

            var response = await serviceSupportClient.GetLocalAuthorityDetailsAsync(localAuthority.Id.Value, CancellationToken.None);
             
            Assert.NotNull(response);
            Assert.NotNull(response.LocalAuthority);
            Assert.NotNull(response.Contact);
            Assert.Equal(response.LocalAuthority.Id!.Value, localAuthority.Id!.Value);
            Assert.Equal(response.LocalAuthority.Name, localAuthority.Name);
            Assert.Equal(response.Contact.Id!.Value, contact.Id!.Value);
            Assert.Equal(response.Contact.Name, contact.Name);
        }
    }
}
