using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project
{
    public class CreateConversionProjectCommandHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldCreateAndReturnProjectId_WhenCommandIsValid(
            [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
            CreateConversionProjectCommandHandler handler,
            CreateConversionProjectCommand command
        )
        {
            mockProjectRepository.GetUserByAdId(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(
                new User { Id = new UserId(Guid.NewGuid()), Team = ProjectTeam.WestMidlands.ToDescription() });

            var now = DateTime.UtcNow;

            var project = Domain.Entities.Project.CreateConversionProject(
                new ProjectId(Guid.NewGuid()),
                new Urn(2),
                now,
                now,
                Domain.Enums.TaskType.Conversion,
                Domain.Enums.ProjectType.Conversion,
                Guid.NewGuid(),
                DateOnly.MinValue,
                true,
                new Domain.ValueObjects.Ukprn(2),
                "region",
                true,
                true,
                DateOnly.MinValue,
                "",
                "",
                "",
                Guid.Empty,
                "",
                null,
                null,
                null);

            // Arrange
            mockProjectRepository.AddAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(project));

            // Act
            await handler.Handle(command, default);

            // Assert
            await mockProjectRepository.Received(1)
                .AddAsync(Arg.Is<Domain.Entities.Project>(s => s.Urn == command.Urn), default);
        }
    }
}