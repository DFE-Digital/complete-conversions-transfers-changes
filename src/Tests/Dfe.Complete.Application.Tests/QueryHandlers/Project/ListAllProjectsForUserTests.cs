using System.Collections.ObjectModel;
using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MediatR;
using MockQueryable;
using Moq;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsForUserTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
        [Frozen] IListAllProjectsForUserQueryService mockListAllProjectsForUserQueryService,
        [Frozen] Mock<ISender> mockSender,
        IFixture fixture)
    {
        //Arrange 
        var mockTrustsClient = new Mock<ITrustsV4Client>();
        
        var handler = new ListAllProjectsForUserQueryHandler(
            mockListAllProjectsForUserQueryService,
            mockTrustsClient.Object,
            mockSender.Object);
        
        
        var userDto = fixture.Create<UserDto>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserByAdIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto?>.Success(userDto));
        
        var mockListAllProjectsForUserQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50);

        var trustDtos = fixture.Create<ObservableCollection<TrustDto>>();
        mockTrustsClient.Setup(service => service.GetByUkprnsAllAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(trustDtos);

        var expected = mockListAllProjectsForUserQueryModels.Select(item =>
                ListAllProjectsForUserQueryResultModel
                    .MapProjectAndEstablishmentToListAllProjectsForUserQueryResultModel(
                        item.Project, item.Establishment, item.Project.OutgoingTrustUkprn.Value.ToString(),
                        item.Project.IncomingTrustUkprn.Value.ToString()))
            .Skip(20).Take(20).ToList();
        
        mockListAllProjectsForUserQueryService.ListAllProjectsForUser(userDto.Id, ProjectState.Active)
            .Returns(mockListAllProjectsForUserQueryModels.BuildMock());

        var query = new ListAllProjectForUserQuery(ProjectState.Active, userDto.ActiveDirectoryUserId) { Page = 1 };

        //Act
        var result = await handler.Handle(query, default);

        //Assert
        Assert.NotNull(result);
    }
}