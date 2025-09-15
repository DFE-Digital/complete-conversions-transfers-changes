using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetConversionTasksData;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.GetTransferTasks
{
    public class GetConversionTasksDataByIdQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldGetAConversionTaskDataById_WhenCommandIsValid(
            [Frozen] ITaskDataReadRepository mockConversionTaskDataRepository,
            [Frozen] IMapper mockMapper,
            GetConversionTasksDataByIdQueryHandler handler,
            GetConversionTasksDataByIdQuery command
        )
        {
            var now = DateTime.UtcNow;

            var ConversionDataTask = new ConversionTasksData(
                new TaskDataId(command.Id!.Value),
                now,
                now);

            // Arrange
            mockConversionTaskDataRepository.ConversionTaskData.Returns(new List<ConversionTasksData> { ConversionDataTask }.AsQueryable().BuildMock());

            mockMapper.Map<ConversionTaskDataDto>(ConversionDataTask).Returns(new ConversionTaskDataDto()
            {
                Id = ConversionDataTask.Id,
                CreatedAt = ConversionDataTask.CreatedAt,
                UpdatedAt = ConversionDataTask.UpdatedAt,
                AcademyDetailsName = ConversionDataTask.AcademyDetailsName,
            });

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value?.Id == command.Id);
        } 

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFailAndReturnErrorMessage_WhenExceptionIsThrown(
            [Frozen] ITaskDataReadRepository mockConversionTaskDataRepository,
            GetConversionTasksDataByIdQueryHandler handler,
            GetConversionTasksDataByIdQuery command
        )
        {
            // Arrange
            var expectedErrorMessage = "Expected Error Message";

            mockConversionTaskDataRepository.ConversionTaskData.Throws(new Exception(expectedErrorMessage));

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Error, expectedErrorMessage);
        }
    }
}
