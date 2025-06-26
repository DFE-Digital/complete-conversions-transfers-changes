using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using AutoMapper;
using Dfe.Complete.Application.Common.Mappers;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;

namespace Dfe.Complete.Application.Tests.Mappers
{
    public class SignificantDateHistoryReasonMapperTests
    {
        private readonly IMapper _mapper;

        public SignificantDateHistoryReasonMapperTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapping>();
            });

            _mapper = config.CreateMapper();
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Map_SignificantDateHistoryReasonToSignificantDateHistoryReasonDto_ShouldMapAllPropertiesCorrectly(Domain.Entities.SignificantDateHistoryReason significantDateHistoryReason)
        {
            // Act
            var significantDateHistoryReasonDto = _mapper.Map<SignificantDateHistoryReasonDto>(significantDateHistoryReason);

            // Assert
            Assert.NotNull(significantDateHistoryReasonDto);
            Assert.Equal(significantDateHistoryReason.Id, significantDateHistoryReasonDto.Id);
            Assert.Equal(significantDateHistoryReason.CreatedAt, significantDateHistoryReasonDto.CreatedAt);
            Assert.Equal(significantDateHistoryReason.UpdatedAt, significantDateHistoryReasonDto.UpdatedAt);
            Assert.Equal(significantDateHistoryReason.ReasonType, significantDateHistoryReasonDto.ReasonType);
            Assert.Equal(significantDateHistoryReason.SignificantDateHistoryId, significantDateHistoryReasonDto.SignificantDateHistoryId);
        }
    }
}
