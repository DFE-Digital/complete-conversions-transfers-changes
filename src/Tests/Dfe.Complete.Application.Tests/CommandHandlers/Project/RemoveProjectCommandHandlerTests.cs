using AutoFixture.Xunit2;
using Dfe.Complete.Application.Common.Exceptions;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Projects.Commands.RemoveProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Moq;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class RemoveProjectCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_InWrongEnvironment_WillThrowError(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<TransferTasksData> mockTransferTaskRepository,
        [Frozen] ICompleteRepository<ConversionTasksData> mockConversionTaskRepository,
        RemoveProjectCommand command
    )
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        IHostEnvironment host = new HostingEnvironment();
        host.EnvironmentName = "Production";

        var handler = new RemoveProjectCommandHandler(
            host,
            mockProjectRepository,
            mockTransferTaskRepository,
            mockConversionTaskRepository,
            unitOfWorkMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotDevOrTestEnvironmentException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}