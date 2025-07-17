using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using AutoMapper;
using Dfe.Complete.Application.Common.Mappers;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;

namespace Dfe.Complete.Application.Tests.Mappers
{
    public class SignificantDateHistoryMapperTests
    {
        private readonly IMapper _mapper;

        public SignificantDateHistoryMapperTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapping>();
            });

            _mapper = config.CreateMapper();
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public void Map_SignificantDateHistoryToSignificantDateHistoryDto_ShouldMapAllPropertiesCorrectly(Domain.Entities.SignificantDateHistory significantDateHistory)
        {
            // Act
            var significantDateHistoryDto = _mapper.Map<SignificantDateHistoryDto>(significantDateHistory);

            // Assert
            Assert.NotNull(significantDateHistoryDto);
            Assert.Equal(significantDateHistory.Id, significantDateHistoryDto.Id);
            Assert.Equal(significantDateHistory.CreatedAt, significantDateHistoryDto.CreatedAt);
            Assert.Equal(significantDateHistory.UpdatedAt, significantDateHistoryDto.UpdatedAt);
            Assert.Equal(significantDateHistory.RevisedDate, significantDateHistoryDto.RevisedDate);
            Assert.Equal(significantDateHistory.PreviousDate, significantDateHistoryDto.PreviousDate);
            Assert.Equal(significantDateHistory.ProjectId, significantDateHistoryDto.ProjectId);
            Assert.Equal(significantDateHistory.UserId, significantDateHistoryDto.UserId);
            Assert.Equal(significantDateHistory.User, significantDateHistoryDto.User);
        }
    }
}
