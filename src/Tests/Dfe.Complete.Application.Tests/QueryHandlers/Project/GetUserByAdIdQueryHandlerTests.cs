using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetUserByAdIdQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation), typeof(UserCustomization))]
        public async Task Handle_ShouldGetAUserDto_WhenCommandIsValid(
            [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
            [Frozen] IMapper mockMapper,
            GetUserByAdIdQueryHandler handler,
            GetUserByAdIdQuery query,
            Domain.Entities.User user
            )
        {
            // Arrange
            mockUserRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.User, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(user);

            var userDto = new UserDto() { ActiveDirectoryUserGroupIds = user.ActiveDirectoryUserGroupIds, FirstName = user.FirstName };

            mockMapper.Map<UserDto>(user).Returns(userDto);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            await mockUserRepository.Received(1).FindAsync(Arg.Any<Expression<Func<Domain.Entities.User, bool>>>(), Arg.Any<CancellationToken>());
            Assert.True(result.IsSuccess);

            var returnedDto = result.Value;

            Assert.Equal(returnedDto.FirstName, userDto.FirstName);
            Assert.Equal(returnedDto.ActiveDirectoryUserGroupIds, userDto.ActiveDirectoryUserGroupIds);
        }


        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation), typeof(UserCustomization))]
        public async Task Handle_ShouldSucceedAndReturnNullWhenUserNotFound_WhenCommandIsValid(
            [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
            GetUserByAdIdQueryHandler handler,
            GetUserByAdIdQuery query
            )
        {
            // Arrange
            mockUserRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.User, bool>>>(), Arg.Any<CancellationToken>())
                .Returns((Domain.Entities.User?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            await mockUserRepository.Received(1).FindAsync(Arg.Any<Expression<Func<Domain.Entities.User, bool>>>(), Arg.Any<CancellationToken>());
            Assert.True(result.IsSuccess);
            Assert.True(result.Value == null);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFailAndReturnError_WhenRepoCallFails(
            [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
            GetUserByAdIdQueryHandler handler,
            GetUserByAdIdQuery query
        )
        {
            // Arrange
            mockUserRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.User, bool>>>(), Arg.Any<CancellationToken>())
                .Throws(new Exception("503"));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            await mockUserRepository.Received(1).FindAsync(Arg.Any<Expression<Func<Domain.Entities.User, bool>>>(), Arg.Any<CancellationToken>());
            Assert.True(!result.IsSuccess);
        }
    }
}
