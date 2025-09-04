using AutoMapper;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Mappers;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.Mappers
{
    public class ContactMapperTests
    {
        private readonly IMapper _mapper;

        public ContactMapperTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapping>();
            });

            config.AssertConfigurationIsValid(); // Ensures all mappings are valid

            _mapper = config.CreateMapper();
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]        
        public async Task Map_ContactToContactDto_ShouldMapAllPropertiesCorrectly(Contact contact)
        {
            // Act
            var contactDto = _mapper.Map<ContactDto>(contact);

            // Assert

            Assert.Multiple(
                () => Assert.NotNull(contactDto),
                () => Assert.Equal(contact.Id, contactDto.Id),
                () => Assert.Equal(contact.ProjectId, contactDto.ProjectId),
                () => Assert.Equal(contact.Name, contactDto.Name),
                () => Assert.Equal(contact.Title, contactDto.Title),
                () => Assert.Equal(contact.Email, contactDto.Email),
                () => Assert.Equal(contact.Phone, contactDto.Phone),
                () => Assert.Equal(contact.Category, contactDto.Category),
                () => Assert.Equal(contact.OrganisationName, contactDto.OrganisationName),
                () => Assert.Equal(contact.Type, contactDto.Type),
                () => Assert.Equal(contact.LocalAuthorityId, contactDto.LocalAuthorityId),
                () => Assert.Equal(contact.EstablishmentUrn, contactDto.EstablishmentUrn)
            );
        }
    }
}
