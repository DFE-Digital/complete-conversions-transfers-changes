using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;
using System.Linq.Expressions;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class UpdateSignificantDateCommandHandlerTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldUpdateProjectSignificantDate_WhenValidInput(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        [Frozen] ICompleteRepository<SignificantDateHistoryReason> mockSignificantDateReasonRepository,
        IFixture fixture)
    {
        // Arrange
        var project = fixture.Create<Domain.Entities.Project>();
        var user = fixture.Create<User>();

        var command = new UpdateSignificantDateCommand(
            project.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
            new Dictionary<SignificantDateReason, string>
            {
                { SignificantDateReason.Buildings, "Reason for change" }
            },
            user.Email
        );

        mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(project);

        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var handler = new UpdateSignificantDateCommandHandler(
            mockProjectRepository,
            mockUserRepository,
            mockSignificantDateReasonRepository
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.SignificantDate, project.SignificantDate);
        await mockSignificantDateReasonRepository.Received(1).AddRangeAsync(Arg.Any<ICollection<SignificantDateHistoryReason>>(), Arg.Any<CancellationToken>());
        await mockProjectRepository.Received(1).UpdateAsync(project, Arg.Any<CancellationToken>());
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserNotFound(
        [Frozen] ICompleteRepository<Domain.Entities.Project> mockProjectRepository,
        [Frozen] ICompleteRepository<User> mockUserRepository,
        [Frozen] ICompleteRepository<SignificantDateHistoryReason> mockSignificantDateReasonRepository,
        IFixture fixture)
    {
        // Arrange
        var project = fixture.Create<Domain.Entities.Project>();

        var command = new UpdateSignificantDateCommand(
            project.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
            new Dictionary<SignificantDateReason, string>
            {
                { SignificantDateReason.Buildings, "Reason" }
            },
            "nonexistent.user@example.com"
        );

        mockProjectRepository.FindAsync(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(project);

        mockUserRepository.FindAsync(Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var handler = new UpdateSignificantDateCommandHandler(
            mockProjectRepository,
            mockUserRepository,
            mockSignificantDateReasonRepository
        );

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<NotFoundException>(exception);
        await mockSignificantDateReasonRepository.DidNotReceive().AddRangeAsync(Arg.Any<ICollection<SignificantDateHistoryReason>>(), Arg.Any<CancellationToken>());
        await mockProjectRepository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Entities.Project>(), Arg.Any<CancellationToken>());
    }
}
