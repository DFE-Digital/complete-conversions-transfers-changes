using AutoFixture.Xunit2;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Domain.ValueObjects;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using NSubstitute.ExceptionExtensions;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Projects.Interfaces;
using MockQueryable;

namespace Dfe.Complete.Application.Tests.QueryHandlers.GetTransferTasks
{
    public class GetTransferTasksDataByIdQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldGetATransferTaskDataById_WhenCommandIsValid(
            [Frozen] ITaskDataReadRepository mockTransferTaskDataRepository,
            [Frozen] IMapper mockMapper,
            GetTransferTasksDataByIdQueryHandler handler,
            GetTransferTasksDataByIdQuery command
        )
        {
            var now = DateTime.UtcNow;

            var transferDataTask = new TransferTasksData(
                new TaskDataId(command.Id!.Value),
                now,
                now,
                true,
                true,
                true
            );

            // Arrange
            mockTransferTaskDataRepository.TransferTaskData
                .Returns(new List<TransferTasksData> { transferDataTask }.AsQueryable().BuildMock());

            mockMapper.Map<TransferTaskDataDto>(transferDataTask).Returns(new TransferTaskDataDto()
            {
                Id = transferDataTask.Id,
                CreatedAt = transferDataTask.CreatedAt,
                UpdatedAt = transferDataTask.UpdatedAt,
                InadequateOfsted = transferDataTask.InadequateOfsted,
                FinancialSafeguardingGovernanceIssues = transferDataTask.FinancialSafeguardingGovernanceIssues,
                OutgoingTrustToClose = transferDataTask.OutgoingTrustToClose
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
            [Frozen] ITaskDataReadRepository mockTransferTaskDataRepository,
            GetTransferTasksDataByIdQueryHandler handler,
            GetTransferTasksDataByIdQuery command
        )
        {
            // Arrange
            var expectedErrorMessage = "Expected Error Message";

            mockTransferTaskDataRepository.TransferTaskData.Throws(new Exception(expectedErrorMessage));

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Error, expectedErrorMessage);
        }
    }
}
