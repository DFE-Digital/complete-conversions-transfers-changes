using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using System.Linq.Expressions;
using Dfe.Complete.Domain.ValueObjects;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using NSubstitute.ExceptionExtensions;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Tests.QueryHandlers.GetTransferTasks
{
    public class GetTransferTasksDataByIdQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldGetATransferTaskDataById_WhenCommandIsValid(
            [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskDataRepository,
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
            mockTransferTaskDataRepository.GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>())
                .Returns(transferDataTask);

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
            await mockTransferTaskDataRepository.Received(1)
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>());
            Assert.True(result.IsSuccess);
            Assert.True(result.Value?.Id == command.Id);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldSucceedAndReturnNullWhenUnfoundTransferTaskDataById_WhenCommandIsValid(
            [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskDataRepository,
            GetTransferTasksDataByIdQueryHandler handler,
            GetTransferTasksDataByIdQuery command
        )
        {
            // Act
            var result = await handler.Handle(command, default);

            // Assert
            await mockTransferTaskDataRepository.Received(1)
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>());
            Assert.True(result.IsSuccess);
            Assert.True(result.Value == null);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFailAndReturnErrorMessage_WhenExceptionIsThrown(
            [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskDataRepository,
            GetTransferTasksDataByIdQueryHandler handler,
            GetTransferTasksDataByIdQuery command
        )
        {
            // Arrange
            var expectedErrorMessage = "Expected Error Message";

            mockTransferTaskDataRepository.GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>())
                .Throws(new Exception(expectedErrorMessage));

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            await mockTransferTaskDataRepository.Received(1)
                .GetAsync(Arg.Any<Expression<Func<TransferTasksData, bool>>>());
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Error, expectedErrorMessage);
        }
    }
}
