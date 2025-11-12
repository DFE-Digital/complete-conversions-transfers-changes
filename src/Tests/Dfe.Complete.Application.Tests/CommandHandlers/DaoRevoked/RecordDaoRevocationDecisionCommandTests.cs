using AutoFixture.Xunit2;
using Dfe.Complete.Application.DaoRevoked.Commands;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.CommandHandlers.DaoRevoked
{
    public class RecordDaoRevocationDecisionCommandTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(ProjectCustomization))]
        public async Task Handle_ShouldDaoRevokedProjectSuccessfully(
            [Frozen] IProjectReadRepository mockProjectReadRepository,
            RecordDaoRevocationDecisionCommandHandler handler,
            Domain.Entities.Project project)
        {
            //Arange
            var command = new RecordDaoRevocationDecisionCommand(project.Id)
            {
                DecisionDate = DateOnly.MinValue,
                MinisterName = "Minister",
                UserId = new UserId(Guid.NewGuid()),
                ReasonNotes = new Dictionary<DaoRevokedReason, string>  {
                    {
                        DaoRevokedReason.SchoolClosedOrClosing, "Closing school"
                    }
                }
            };
            var queryableProjects = new List<Domain.Entities.Project> { project }.AsQueryable().BuildMock();
            mockProjectReadRepository.Projects.Returns(queryableProjects);

            // Action
            var result = await handler.Handle(command, CancellationToken.None);

            //Asert
            Assert.True(result.IsSuccess);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFailAndReturnErrorMessage_WhenExceptionIsThrown(
            [Frozen] IProjectReadRepository mockProjectRepository,
            RecordDaoRevocationDecisionCommandHandler handler,
            RecordDaoRevocationDecisionCommand command
        )
        {
            // Arrange
            var expectedErrorMessage = "Expected Error Message";

            mockProjectRepository.Projects.Throws(new Exception(expectedErrorMessage));

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(result.Error, expectedErrorMessage);
        }
    }
}
