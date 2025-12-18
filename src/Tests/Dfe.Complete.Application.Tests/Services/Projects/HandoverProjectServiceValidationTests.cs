using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MockQueryable;
using NSubstitute;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Tests.Services.Projects;

public class HandoverProjectServiceValidationTests
{
    [Fact]
    public async Task ValidateUrnAsync_WhenProjectAlreadyExists_ThrowsWithLegacyMessage()
    {
        var sender = Substitute.For<MediatR.ISender>();
        var projectRepository = Substitute.For<ICompleteRepository<Project>>();
        var conversionTaskRepository = Substitute.For<ICompleteRepository<ConversionTasksData>>();
        var transferTaskRepository = Substitute.For<ICompleteRepository<TransferTasksData>>();
        var keyContactWriteRepository = Substitute.For<Dfe.Complete.Application.KeyContacts.Interfaces.IKeyContactWriteRepository>();

        var urn = 123456;
        var existingProject = new Project
        {
            Urn = new Urn(urn),
            State = ProjectState.Active
        };

        projectRepository.Query().Returns(new[] { existingProject }.AsQueryable().BuildMock());

        var sut = new HandoverProjectService(
            sender,
            projectRepository,
            conversionTaskRepository,
            transferTaskRepository,
            keyContactWriteRepository);

        var ex = await Assert.ThrowsAsync<UnprocessableContentException>(() => sut.ValidateUrnAsync(urn, CancellationToken.None));
        Assert.Equal($"A project with the urn: {urn} already exists", ex.Message);
    }

    [Fact]
    public async Task ValidateTrnAndTrustNameAsync_WhenExistingTrnHasDifferentName_ThrowsValidationExceptionWithLegacyMessage()
    {
        var sender = Substitute.For<MediatR.ISender>();
        var projectRepository = Substitute.For<ICompleteRepository<Project>>();
        var conversionTaskRepository = Substitute.For<ICompleteRepository<ConversionTasksData>>();
        var transferTaskRepository = Substitute.For<ICompleteRepository<TransferTasksData>>();
        var keyContactWriteRepository = Substitute.For<Dfe.Complete.Application.KeyContacts.Interfaces.IKeyContactWriteRepository>();

        projectRepository
            .FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Project, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Project>(new Project
            {
                NewTrustReferenceNumber = "TR12345",
                NewTrustName = "Big new trust"
            })!);

        var sut = new HandoverProjectService(
            sender,
            projectRepository,
            conversionTaskRepository,
            transferTaskRepository,
            keyContactWriteRepository);

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            sut.ValidateTrnAndTrustNameAsync("TR12345", "New trust name", CancellationToken.None));

        Assert.Equal(
            "A trust with this TRN already exists. It is called Big new trust. Check the trust name you have entered for this conversion/transfer.",
            ex.Message);
    }

    [Fact]
    public async Task ValidateTrnAndTrustNameAsync_WhenExistingTrnHasSameName_DoesNotThrow()
    {
        var sender = Substitute.For<MediatR.ISender>();
        var projectRepository = Substitute.For<ICompleteRepository<Project>>();
        var conversionTaskRepository = Substitute.For<ICompleteRepository<ConversionTasksData>>();
        var transferTaskRepository = Substitute.For<ICompleteRepository<TransferTasksData>>();
        var keyContactWriteRepository = Substitute.For<Dfe.Complete.Application.KeyContacts.Interfaces.IKeyContactWriteRepository>();

        projectRepository
            .FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Project, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Project>(new Project
            {
                NewTrustReferenceNumber = "TR12345",
                NewTrustName = "Big new trust"
            })!);

        var sut = new HandoverProjectService(
            sender,
            projectRepository,
            conversionTaskRepository,
            transferTaskRepository,
            keyContactWriteRepository);

        await sut.ValidateTrnAndTrustNameAsync("TR12345", "Big new trust", CancellationToken.None);
    }
}

