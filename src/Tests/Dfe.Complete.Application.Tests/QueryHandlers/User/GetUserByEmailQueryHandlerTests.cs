using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryHandlers.User;

public class GetUserByEmailQueryHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(UserCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnSuccess_WhenUserIsFound(
        [Frozen] ICompleteRepository<Domain.Entities.User> mockUserRepository,
        [Frozen] IMapper mockMapper,
        GetUserByEmailQueryHandler handler,
        Domain.Entities.User user,
        GiasEstablishment establishment,
        IFixture fixture)
    {
        establishment.Urn ??= new Urn(123456);
        // Arrange
        var conversionProject = fixture.Customize(new ProjectCustomization
        {
            Urn = establishment.Urn,
            Type = ProjectType.Conversion,
            AssignedToId = user.Id
        }).Create<Domain.Entities.Project>();

        var transferProject = fixture.Customize(new ProjectCustomization
        {
            Urn = establishment.Urn,
            Type = ProjectType.Transfer,
            AssignedToId = user.Id
        }).Create<Domain.Entities.Project>();

        var projectList = new List<Domain.Entities.Project> { conversionProject, transferProject };
        user.ProjectAssignedTos = projectList;
        user.Email ??= "test@email.com";

        mockUserRepository
            .FindAsync(Arg.Any<Expression<Func<Domain.Entities.User, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var userDto = new UserDto
        {
            ActiveDirectoryUserGroupIds = user.ActiveDirectoryUserGroupIds, FirstName = user.FirstName,
            LastName = user.LastName, Email = user.Email
        };

        mockMapper.Map<UserDto>(user).Returns(userDto);

        var query = new GetUserByEmailQuery(user.Email);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(user.FullName, result.Value.FullName);
        Assert.Equal(user.Email, result.Value.Email);
    }
}