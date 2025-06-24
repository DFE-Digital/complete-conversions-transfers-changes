using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MediatR;
using NSubstitute;

namespace Dfe.Complete.Tests.Models;

public class BaseProjectPageModelTests
{
    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task UpdateCurrentProject_ThrowsInvalidDataException_WhenProjectIdIsInvalid(
        [Frozen] ISender sender)
    {
        // Arrange
        var model = new TestBaseProjectPageModel(sender)
        {
            ProjectId = "not-a-guid"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(() => model.UpdateCurrentProject());
        Assert.Contains("not-a-guid", exception.Message);
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task UpdateCurrentProject_ThrowsNotFoundException_WhenResultIsFailure(
        [Frozen] ISender sender,
        Guid guid)
    {
        // Arrange
        var model = new TestBaseProjectPageModel(sender)
        {
            ProjectId = guid.ToString()
        };

        var failedResult = Result<ProjectDto?>.Failure("Project not found");

        sender.Send(Arg.Any<GetProjectByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(failedResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => model.UpdateCurrentProject());
        Assert.Contains(guid.ToString(), exception.Message);
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task UpdateCurrentProject_ThrowsNotFoundException_WhenResultValueIsNull(
        [Frozen] ISender sender,
        Guid guid)
    {
        // Arrange
        var model = new TestBaseProjectPageModel(sender)
        {
            ProjectId = guid.ToString()
        };

        var nullResult = Result<ProjectDto?>.Success(null!); // simulate null value

        sender.Send(Arg.Any<GetProjectByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(nullResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => model.UpdateCurrentProject());
        Assert.Contains(guid.ToString(), exception.Message);
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation), typeof(DateOnlyCustomization))]
    public async Task UpdateCurrentProject_SetsProject_WhenResultIsSuccessful(
        [Frozen] ISender sender,
        Guid guid,
        ProjectDto projectDto)
    {
        // Arrange
        var model = new TestBaseProjectPageModel(sender)
        {
            ProjectId = guid.ToString()
        };

        var successResult = Result<ProjectDto?>.Success(projectDto);

        sender.Send(Arg.Any<GetProjectByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(successResult);

        // Act
        await model.UpdateCurrentProject();

        // Assert
        Assert.Equal(projectDto, model.Project);
    }
}

public class TestBaseProjectPageModel(ISender sender) : BaseProjectPageModel(sender)
{
}