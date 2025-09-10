using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using AutoMapper;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Application.Mappers;

namespace Dfe.Complete.Application.Tests.Mappers
{
    public class EstablishmentMapperTests
    {
        private readonly IMapper _mapper;

        public EstablishmentMapperTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapping>();
            });

            _mapper = config.CreateMapper();
        }
        
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public void Map_GiasEstablishmentToEstablishmentDto_ShouldMapCommonPropertiesCorrectly(Domain.Entities.GiasEstablishment giasEstablishment)
        {
            // Act
            var dto = _mapper.Map<EstablishmentDto>(giasEstablishment);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(giasEstablishment.Urn?.ToString(), dto.Urn);
            Assert.Equal(giasEstablishment.Ukprn?.ToString(), dto.Ukprn);
            Assert.Equal(giasEstablishment.Name, dto.Name);
            Assert.Equal(giasEstablishment.EstablishmentNumber, dto.EstablishmentNumber);
            Assert.Equal(giasEstablishment.LocalAuthorityCode, dto.LocalAuthorityCode);
            Assert.Equal(giasEstablishment.LocalAuthorityName, dto.LocalAuthorityName);
            Assert.Equal(giasEstablishment.AgeRangeLower?.ToString(), dto.StatutoryLowAge);
            Assert.Equal(giasEstablishment.AgeRangeUpper?.ToString(), dto.StatutoryHighAge);
            Assert.Equal(giasEstablishment.AddressPostcode, dto.Address?.Postcode);
            Assert.Equal(giasEstablishment.DioceseName, dto.Diocese?.Name);
            Assert.Equal(giasEstablishment.TypeName, dto.EstablishmentType?.Name);
            Assert.Equal(giasEstablishment.PhaseName, dto.PhaseOfEducation?.Name);
            Assert.Equal(giasEstablishment.RegionName, dto.Gor?.Name);
            Assert.Equal(giasEstablishment.ParliamentaryConstituencyName, dto.ParliamentaryConstituency?.Name);
        }
    }
}
