using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.DaoRevoked.Interfaces;
using Dfe.Complete.Application.DaoRevoked.Models;
using Dfe.Complete.Application.DaoRevoked.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryHandlers.DaoRevoked
{
    public class GetDaoRevocationByProjectIdQueryTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(DaoRevocationCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnSuccess_WhenProjectIsFound(
            [Frozen] IDaoRevocationReadRepository mockDaoRevocationReadRepository,
            [Frozen] IMapper mockMapper,
            GetDaoRevocationByProjectIdQueryHandler handler,
            DaoRevocation daoRevocation,
            DaoRevocationDto mappedDaoRevocation)
        {
            // Arrange
            var queryableProjects = new List<DaoRevocation> { daoRevocation }.AsQueryable().BuildMock();
            mockDaoRevocationReadRepository.DaoRevocations.Returns(queryableProjects);
            mockMapper.Map<DaoRevocationDto>(Arg.Any<DaoRevocation>()).Returns(mappedDaoRevocation);

            var query = new GetDaoRevocationByProjectIdQuery(daoRevocation.ProjectId!);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(mappedDaoRevocation, result.Value);
        }
    }
}
